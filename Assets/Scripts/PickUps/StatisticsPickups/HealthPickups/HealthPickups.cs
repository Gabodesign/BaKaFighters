using UnityEngine;

public class HealthPickups : MonoBehaviour
{
    [Header("Valore cura")]
    public string playerTag = "Player";
    public float healAmount = 10f;
    private bool consumato;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (consumato) return;

        if (collision.gameObject.CompareTag(playerTag)) // Usiamo la variabile playerTag per sicurezza
        {
            Debug.Log("Ho colpito " + collision.gameObject.name);
            var player = collision.gameObject.GetComponent<PlayerController>();

            if (player != null)
            {
                // 1. Diamo i 10 punti al Player
                player.AddHealth(healAmount);

                // REMOVED: Cancellata la riga della UI da qui! Ci pensa già il Player.

                consumato = true;
                Destroy(gameObject);
            }
        }
    }
}