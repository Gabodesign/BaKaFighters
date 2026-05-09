using UnityEngine;

public abstract class EnemiesConfig : MonoBehaviour
{

    [SerializeField] protected float moveSpeed;
    [SerializeField] protected int scoreValue;
    [SerializeField] protected bool useTouchDamage;
    [SerializeField] protected float touchDamage;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float health;
    [SerializeField] protected bool canMove;
    [SerializeField] protected HitEffect hitDamage;
    [SerializeField] protected Color colorDamage = new Color(1f, 0f, 0f, 1f);
    [SerializeField] public bool canShot;

    public enum DIRECTION
    {
        Forward,
        Backward,
        Top,
        Bottom
    }
    public DIRECTION dir = DIRECTION.Forward;

    protected virtual void Awake()
    {
        hitDamage = GetComponent<HitEffect>();
    }

    public virtual void Start()
    {
        health = maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyEnemy"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            TakeDamage(collision.gameObject.GetComponent<Projectile2D>().damage);
        }

        if (collision.gameObject.CompareTag("Player") && useTouchDamage)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            player.TakeDamage(touchDamage);
        }
    }
    public virtual void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            ScoreManager.Instance.AddPoint(scoreValue);
            Destroy(gameObject);
        }
        hitDamage.FlashOnce(colorDamage, hitDamage.defaultDuration);
    }
}
