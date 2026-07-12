using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected EnemyData data;

    // Lo stato del nemico specifico in scena sta qui!
    protected float currentHealth;
    protected HitEffect hitDamage;
    public bool CanShoot
    {
        get { return data != null && data.canShoot; }
    }

    public GameObject BulletPrefab
    {
        get { return data.bulletPrefab; }
    }

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
        // Il componente lo salviamo sul MonoBehaviour, non nel Data
        hitDamage = GetComponent<HitEffect>();
    }

    public virtual void Start()
    {
        // Inizializziamo la vita corrente del singolo nemico con la massima del data
        currentHealth = data.maxHealth;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("DestroyEnemy"))
        {
            Destroy(gameObject);
        }

        if (collision.gameObject.CompareTag("Bullet"))
        {
            // Nota: Assicurati che Projectile2D esista e abbia .damage
            TakeDamage(collision.gameObject.GetComponent<Projectile2D>().damage);
        }

        if (collision.gameObject.CompareTag("Player") && data.useTouchDamage)
        {
            var player = collision.gameObject.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(data.touchDamage);
            }
        }
    }

    public virtual void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (hitDamage != null)
        {
            hitDamage.FlashOnce(data.colorDamage, hitDamage.defaultDuration);
        }

        if (currentHealth <= 0)
        {
            // Accedi a scoreValue tramite "data"
            GameManager.Instance.AddPoint(data.scoreValue);
            Destroy(gameObject);
        }
    }
}