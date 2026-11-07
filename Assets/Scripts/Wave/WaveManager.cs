using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;
    public GameObject[] enemiesPrefab;
    public enum WaveState { Scrolling, Warning, Combat, Completed, Continue }
    [Header("Riferimenti")]
    [SerializeField] private WaveUI waveUI;

    private WaveState state = WaveState.Scrolling;

    public WaveState State => state;
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

    // Metodo per spawnare un nemico in un punto di spawn specifico
    private GameObject SpawnEnemy(WaveSpawnPoint point)
    {
        if (point == null)
        {
            Debug.LogWarning("WaveManager: punto di spawn vuoto nella lista, lo salto.");
            return null;
        }

        GameObject prefab = RandomEnemy();
        if (prefab == null)
        {
            Debug.LogWarning("WaveManager: la lista Enemy Prefabs è vuota.");
            return null;
        }

        GameObject enemy = Instantiate(prefab, point.transform.position, point.transform.rotation);
        point.ApplyTo(enemy);
        return enemy;
    }

    //prendiamo random i nemici dalla lista.
    private GameObject RandomEnemy()
    {
        if (enemiesPrefab == null || enemiesPrefab.Length == 0) return null;
        return enemiesPrefab[Random.Range(0, enemiesPrefab.Length)];
    }
}