using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance;

    public int currentWave = 0;
    public int enemiesRemaining = 0;
    public bool waveInProgress = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartNextWave();
    }

    public void StartNextWave()
    {
        if (waveInProgress) return;

        currentWave++;
        waveInProgress = true;

        // Esempio: calcola i nemici in base all'ondata
        enemiesRemaining = 3 + (currentWave * 2);
        Debug.Log("Inizio Ondata: " + currentWave);

        // Chiama il tuo spawner per generare 'enemiesRemaining' nemici
    }

    public void EnemyDefeated()
    {
        enemiesRemaining--;

        if (enemiesRemaining <= 0)
        {
            WaveComplete();
        }
    }

    private void WaveComplete()
    {
        waveInProgress = false;
        Debug.Log("Ondata completata!");

        // Attendi qualche secondo prima di avviare la wave successiva
        Invoke("StartNextWave", 3.0f);
    }
}