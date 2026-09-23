using UnityEngine;

public class CircleMovement : MonoBehaviour, IEnemyMovement
{
    private Enemy enemy;
    [SerializeField] private float speed = 3f;
    [SerializeField] private float radius = 1f;

    private float timeCounter = 0f;
    private Vector2 centerPoint;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void Start()
    {
        // Salviamo la posizione di spawn come centro del cerchio
        centerPoint = transform.localPosition;
    }

    public void Move()
    {
        timeCounter += Time.deltaTime * speed;

        float x = Mathf.Cos(timeCounter) * radius;
        float y = Mathf.Sin(timeCounter) * radius;

        transform.localPosition = centerPoint + new Vector2(x, y);
    }
}