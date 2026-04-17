using UnityEngine;

public class Projectile2D : MonoBehaviour
{
    [Header("Projectile")]
    [SerializeField] public float maxLifetime = 3f;  
    [SerializeField] public float speed = 10f;
    [SerializeField] public float damage;
    
    public void Update()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {    
        if (collision.gameObject.layer == LayerMask.NameToLayer("Enemies") || collision.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            Destroy(gameObject);
        }
    }
}
