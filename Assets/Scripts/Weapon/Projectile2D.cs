using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] public float maxLifetime = 3f;  
    [SerializeField] public float speed = 10f;
    [SerializeField] public float damage;

    private void Start()
    {
        Destroy(gameObject, maxLifetime);
    }

    public void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {    
        
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.CompareTag("Foreground") || collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
        }

    }
    public void Launch(Vector2 dir, float speed, float damage)
    {
        this.damage = damage;
        transform.Translate(dir * speed * Time.deltaTime);
    }
}
