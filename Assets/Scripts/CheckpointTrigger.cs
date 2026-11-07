using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CheckpointTrigger : MonoBehaviour
{
    private bool used;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;

        used = true;
        GameManager.Instance.SaveCheckpoint();
    }
}
