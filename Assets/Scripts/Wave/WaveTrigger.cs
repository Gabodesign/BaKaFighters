using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class WaveTrigger : MonoBehaviour
{
    [SerializeField] private int waveIndex;
    [SerializeField] private List<WaveRound> rounds = new List<WaveRound>();


    public int WaveIndex => waveIndex;
    private Collider2D ownCollider;
    private Collider2D stopCollider;

    private void Awake()
    {
        // Il livello si muove con il transform: un Rigidbody2D cinematico rende affidabili gli eventi di trigger
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        ownCollider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        GameObject stop = GameObject.FindWithTag("StopLevel");
        if (stop != null) stopCollider = stop.GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player)"))
        {

        }
        if (other.CompareTag("StopLevel"))
        {
            // Disabilita il trigger per evitare che venga attivato più volte
            ownCollider.enabled = false;
            // Avvia la wave
            //WaveManager.Instance.StartNextWave();
        }
    }
}

[System.Serializable]
public class WaveRound
{
    public float startDelay = 0f;
    public List<SpawnEntry> spawns = new List<SpawnEntry>();
}

[System.Serializable]
public class SpawnEntry
{
    public Transform spawnPoint;
    public float spawnDelay = 0f;
}
