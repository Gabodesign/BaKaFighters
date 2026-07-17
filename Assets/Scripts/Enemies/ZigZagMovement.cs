using UnityEngine;

public class ZigZagMovement : MonoBehaviour, IEnemyMovement 
{
    public float amplitude = 1f; 
    public float frequency = 1f; 

    public void Move()
    { 
        float y = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.position += new Vector3(0, y, 0) * Time.deltaTime;

    }
    
}
