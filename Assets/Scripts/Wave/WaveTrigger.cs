using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private int waveIndex;
    [SerializeField] private List<WaveRound> rounds = new List<WaveRound>();
    public int WaveIndex => waveIndex;
    public List<WaveRound> Rounds => rounds;
    private bool triggered;
    private bool warned;

    private void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }
    private void Awake()
    {
        // Il livello si muove con il transform: un Rigidbody2D cinematico rende affidabili gli eventi di trigger
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (triggered) return;

        // 1. Il Player tocca il box: parte il Warning, che resta acceso finché la wave non inizia
        if (!warned && other.CompareTag("Player"))
        {
            warned = true;
            if (WaveManager.Instance != null)
                WaveManager.Instance.BeginWarning(this);
            return;
        }

        // 2. Il box tocca lo StopLevel: lo scroll si ferma, il Warning sparisce e parte la wave
        if (other.CompareTag("StopLevel"))
        {
            triggered = true;
            if (WaveManager.Instance != null)
                WaveManager.Instance.StartWave(this);
        }
    }
}

[System.Serializable]
public class WaveRound
{
    public float startDelay = 0f;
    public float spawnInterval = 0.5f;
    public int enemyCount = 0;
    public GameObject[] enemyPrefabs;
    public List<WaveSpawnPoint> spawns = new List<WaveSpawnPoint>();
}
