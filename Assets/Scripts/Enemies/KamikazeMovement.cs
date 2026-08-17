using UnityEngine;

public class KamikazeMovement : MonoBehaviour,IEnemyMovement
{
    private Enemy enemy;
    [SerializeField] private float detectionRadius = 12f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }
    public void Move()
    {
        //controllo se il player è nel raggio di rilevamento
        if (enemy != null) 
        {
            //movimento Kaminkaze verso il player
            Vector2 direction = ((Vector2)enemy.PlayerTransform.position - enemy.Rb.position).normalized;
            enemy.Rb.linearVelocity = direction * enemy.MoveSpeed;

            Debug.Log("Kamikaze moving towards player");

        }
    }

}
