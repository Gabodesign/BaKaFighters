using UnityEngine;

public class HealthPickups : MonoBehaviour
{
    [Header("Valore cura")]
    public string playerTag = "Player";
    public float healAmount = 10f;
    private bool consumato;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (consumato) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Ho colpito " + collision.gameObject.name);
            var health = collision.gameObject.GetComponent<PlayerController>();

            health.AddHealth(healAmount);
            consumato = true;
            Destroy(gameObject);
        }

        
    }
}
