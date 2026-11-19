using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public GameObject[] enemiesPrefabs;
    public enum WaveState { Scrolling, Warning, Combat, Completed, Continue }
    [Header("Riferimenti")]
    [SerializeField] private WaveUI waveUI;
    [SerializeField] private Transform enemiesParent;
    [SerializeField] private float spawnZ = 10f;
    [SerializeField] private float waveCompleteDuration = 2.5f;
    [SerializeField] private float continueDuration = 3f;

    [SerializeField] private float respawnDelay = 1f;
    private WaveState state = WaveState.Scrolling;

    // Stato dell'ondata in corso
    private WaveRound currentRound;
    private int killedCount;
    private int totalCount;
    public WaveState State => state;
    private int aliveEnemies;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        // I contatori si vedono solo durante un'area di ondate
        if (waveUI != null)
        {
            waveUI.HideWaves();
            waveUI.HideEnemies();
        }
    }

    private bool IsAlreadyCompleted(WaveTrigger trigger)
    {
        return GameManager.Instance != null && trigger.WaveIndex < GameManager.Instance.WavesCompleted;
    }

    public void BeginWarning(WaveTrigger trigger)
    {
        if (IsAlreadyCompleted(trigger) || state != WaveState.Scrolling) return;

        state = WaveState.Warning;
        AudioManager.instance.PlaySFX(AudioManager.instance != null ? AudioManager.instance.warning : null);
        if (waveUI != null) waveUI.StartWarning();
    }

    public void StartWave(WaveTrigger trigger)
    {
        if (IsAlreadyCompleted(trigger)) return;
        // Va bene sia dopo il Warning sia se il trigger non aveva una distanza di avviso
        if (state != WaveState.Warning && state != WaveState.Scrolling) return;

        StartCoroutine(WaveRoutine(trigger));
    }

    private IEnumerator WaveRoutine(WaveTrigger trigger)
    {
        // 1. Il livello si ferma e il Warning sparisce
        if (LevelScroller.Instance != null) LevelScroller.Instance.SetScrolling(false);
        if (waveUI != null) waveUI.StopWarning();

        // 2. Le ondate di quest'area, una dopo l'altra
        state = WaveState.Combat;
        state = WaveState.Combat;
        int roundCount = trigger.Rounds.Count;
        for (int i = 0; i < roundCount; i++)
        {
            if (waveUI != null)
            {
                waveUI.SetWaves(i + 1, roundCount);
                waveUI.ShowWaves();
            }
            yield return RoundRoutine(trigger.Rounds[i]);
        }

        // 3. Wave Complete, poi il checkpoint
        state = WaveState.Completed;
        if (waveUI != null)
        {
            waveUI.HideWaves();
            waveUI.HideEnemies();
        }
        if (GameManager.Instance != null)
        {
            GameManager.Instance.WavesCompleted = Mathf.Max(GameManager.Instance.WavesCompleted, trigger.WaveIndex + 1);
        }

        AudioManager.instance.PlaySFX(AudioManager.instance != null ? AudioManager.instance.waveComplete : null);
        if (waveUI != null) yield return waveUI.ShowWaveComplete(waveCompleteDuration);
        else yield return new WaitForSeconds(waveCompleteDuration);

        if (GameManager.Instance != null) GameManager.Instance.SaveCheckpoint();

        // 4. Il livello riparte mentre compare Continue
        state = WaveState.Continue;
        if (LevelScroller.Instance != null) LevelScroller.Instance.SetScrolling(true);
        if (waveUI != null) yield return waveUI.ShowContinue(continueDuration);
        else yield return new WaitForSeconds(continueDuration);

        state = WaveState.Scrolling;
    }
    private IEnumerator RoundRoutine(WaveRound round)
    {
        if (round.startDelay > 0f) yield return new WaitForSeconds(round.startDelay);

        if (round.spawns.Count == 0)
        {
            Debug.LogWarning("WaveManager: quest'ondata non ha punti di spawn, la salto.");
            yield break;
        }

        // Quanti nemici: il valore scelto, oppure uno per punto se è 0
        int toSpawn = round.enemyCount > 0 ? round.enemyCount : round.spawns.Count;
        currentRound = round;
        totalCount = toSpawn;
        killedCount = 0;
        aliveEnemies = 0;
        if (waveUI != null)
        {
            waveUI.SetEnemies(0, totalCount);
            waveUI.ShowEnemies();
        }

        for (int i = 0; i < toSpawn; i++)
        {
            if (i > 0 && round.spawnInterval > 0f)
                yield return new WaitForSeconds(round.spawnInterval);

            // I punti si riutilizzano a rotazione se i nemici sono più dei punti
            WaveSpawnPoint point = round.spawns[i % round.spawns.Count];
            GameObject enemy = SpawnEnemy(point, round.enemyPrefabs);
            if (enemy == null)
            {
                totalCount--;
                if (waveUI != null) waveUI.SetEnemies(killedCount, totalCount);
                continue;
            }

            TrackEnemy(enemy, point);
        }

        // Si aspetta che non resti nessun nemico: i nemici usciti dalla zona ricompaiono, quindi contano come ancora vivi
        yield return new WaitUntil(() => aliveEnemies <= 0);
    }

    // Segue il nemico: se muore conta come ucciso, se tocca un DestroyEnemy ricompare dallo stesso punto
    private void TrackEnemy(GameObject enemy, WaveSpawnPoint point)
    {
        aliveEnemies++;
        WaveEnemy tracker = enemy.AddComponent<WaveEnemy>();
        tracker.Destroyed += wave =>
        {
            aliveEnemies--;

            if (wave.Escaped)
            {
                // Uscito dalla zona: non è un'uccisione, né per il contatore né per il punteggio
                StartCoroutine(RespawnRoutine(point));
                return;
            }

            killedCount++;
            if (waveUI != null) waveUI.SetEnemies(killedCount, totalCount);
        };
    }

    private IEnumerator RespawnRoutine(WaveSpawnPoint point)
    {
        // Mentre aspetta il nemico conta come ancora vivo, così l'ondata non finisce
        aliveEnemies++;
        yield return new WaitForSeconds(respawnDelay);
        aliveEnemies--;

        GameObject enemy = SpawnEnemy(point, currentRound.enemyPrefabs);
        if (enemy == null)
        {
            totalCount--;
            if (waveUI != null) waveUI.SetEnemies(killedCount, totalCount);
            yield break;
        }

        TrackEnemy(enemy, point);
    }






    // Metodo per spawnare un nemico in un punto di spawn specifico
    private GameObject SpawnEnemy(WaveSpawnPoint point, GameObject[] roundPrefabs)
    {
        if (point == null)
        {
            Debug.LogWarning("WaveManager: punto di spawn vuoto nella lista, lo salto.");
            return null;
        }

        GameObject prefab = PickRandomEnemy(roundPrefabs);
        if (prefab == null)
        {
            Debug.LogWarning("WaveManager: nessun nemico da creare, riempi la lista dell'ondata o quella del WaveManager.");
            return null;
        }

        Vector3 pos = point.transform.position;
        pos.z = spawnZ;
        GameObject enemy = Instantiate(prefab, pos, point.transform.rotation, enemiesParent);
        point.ApplyTo(enemy);
        SetEnemyLayer(enemy);
        return enemy;
    }

    //impostiamo li layer dei nemici 
    private void SetEnemyLayer(GameObject enemy)
    {
        int layer = LayerMask.NameToLayer("Enemies");
        if(layer < 0)
        {
            Debug.LogWarning("WaveManager: il layer 'Enemies' non esiste, non posso impostarlo ai nemici.");
            return;
        }
        foreach(Transform transform in enemy.GetComponentInChildren<Transform>(true))
        {
            transform.gameObject.layer = layer;
        }
    }


    //prendiamo random i nemici dalla lista.
    private GameObject PickRandomEnemy(GameObject[] roundPrefabs)
    {
        GameObject[] pool = (roundPrefabs != null && roundPrefabs.Length > 0) ? roundPrefabs : enemiesPrefabs;
        if (pool == null || pool.Length == 0) return null;
        return pool[Random.Range(0, pool.Length)];
    }
}