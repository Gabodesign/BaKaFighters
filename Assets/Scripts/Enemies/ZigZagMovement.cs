using UnityEngine;

public class ZigZagMovement : MonoBehaviour, IEnemyMovement 
{
    private Enemy enemy;
    public float amplitude = 5f; 
    public float frequency = 10f;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    public void Move()
    { 

        float zigZag = Mathf.Sin(Time.time * frequency) * amplitude;

        if (enemy != null)
        {
            if(enemy.dir == Enemy.DIRECTION.Forward)
                transform.position += new Vector3(-1, zigZag * enemy.MoveSpeed, 0) * Time.deltaTime;

        }//Debug.Log("ZigZagMovement: Moving in a zig-zag pattern.");
    }
    
}
