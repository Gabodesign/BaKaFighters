using UnityEngine;

public class ZigZagMovement : MonoBehaviour, IEnemyMovement 
{
    private Enemy enemy;
    [SerializeField] private float amplitude = 5f;
    [SerializeField] private float frequency = 10f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Move()
    { 

        float zigZag = Mathf.Sin(Time.time * frequency) * amplitude;

        if (enemy != null)
        {
            // Avanza nella direzione scelta e oscilla sull'asse perpendicolare
            Vector3 move = enemy.dir switch
            {
                Enemy.DIRECTION.Forward => new Vector3(-1, zigZag * enemy.MoveSpeed, 0),
                Enemy.DIRECTION.Backward => new Vector3(1, zigZag * enemy.MoveSpeed, 0),
                Enemy.DIRECTION.Top => new Vector3(zigZag * enemy.MoveSpeed, 1, 0),
                _ => new Vector3(zigZag * enemy.MoveSpeed, -1, 0)
            };
            transform.position += move * Time.deltaTime;

        }
    }
    
}
