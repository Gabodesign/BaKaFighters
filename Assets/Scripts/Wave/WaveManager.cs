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
    private WaveState state = WaveState.Scrolling;
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
        int enemiesTotal = toSpawn;
        int killed = 0;
        aliveEnemies = 0;
        if (waveUI != null)
        {
            waveUI.SetEnemies(0, enemiesTotal);
            waveUI.ShowEnemies();
        }

        for (int i = 0; i < toSpawn; i++)
        {
            if (i > 0 && round.spawnInterval > 0f)
                yield return new WaitForSeconds(round.spawnInterval);

            // I punti si riutilizzano a rotazione se i nemici sono più dei punti
            WaveSpawnPoint point = round.spawns[i % round.spawns.Count];
            GameObject enemy = SpawnEnemy(point, round.enemyPrefabs);
            if (enemy == null) { enemiesTotal--; continue; }

            aliveEnemies++;
            WaveEnemy tracker = enemy.AddComponent<WaveEnemy>();
            tracker.Destroyed += _ =>
            {
                aliveEnemies--;
                killed++;
                if (waveUI != null) waveUI.SetEnemies(killed, enemiesTotal);
            };
        }

        // Si aspetta che non resti nessun nemico (uccisi o usciti dallo schermo)
        yield return new WaitUntil(() => aliveEnemies <= 0);
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
        return enemy;
    }

    //prendiamo random i nemici dalla lista.
    private GameObject PickRandomEnemy(GameObject[] roundPrefabs)
    {
        GameObject[] pool = (roundPrefabs != null && roundPrefabs.Length > 0) ? roundPrefabs : enemiesPrefabs;
        if (pool == null || pool.Length == 0) return null;
        return pool[Random.Range(0, pool.Length)];
    }
}