using Spine.Unity;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using static WeaponsConfig;
public class PlayerController : MonoBehaviour
{
    
    [Header("Movement Settings")]            // Sezione per i parametri di movimento
    public float moveSpeed = 5f;

    [Header("Spine character")]              // Sezione per riferimenti legati al personaggio Spine
    private SkeletonAnimation skeletonAnimation;
    [SerializeField] private string[] flyAnimations = new string[5];
    [SerializeField] private string[] shotAnimations = new string[3];
    private string currentAnim;                    
    private Vector2 moveInput;                   
    public bool isShooting = false;
    public Color colorDamage = new Color(1f, 0f, 0f, 1f);
    public Color colorHealth = new Color(0f, 1f, 0f, 1f);
    [Header("Component Player Health")]   
    [SerializeField] public float health;    
    [SerializeField] public float maxHealth; 

    [Header("Component Player Shield")]   
    [SerializeField] public float shield;    
    [SerializeField] public float maxShield; 

    [Header("Component Player Ki")]       
    [SerializeField] public float ki;        
    [SerializeField] public float maxKi;

    [Header("Armi")]
    [SerializeField] private WeaponsConfig[] weaponConfigs;
    [SerializeField] private WeaponType startingWeapon = WeaponType.Bullet;

    [Header("Effetto")]
    [SerializeField] protected HitEffect hitEffect;

    private BulletArm bulletArm;
    private bool gameover;
    public Transform firePoint;
    private Rigidbody2D rb;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        hitEffect = GetComponent<HitEffect>();


        
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            
            InputManager.Instance.OnMove += HandleMoveInput;
            InputManager.Instance.OnFire += ShotPressed;
            InputManager.Instance.OnFireCanceled += ShotReleased;
        }
    }
    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            
            InputManager.Instance.OnMove -= HandleMoveInput;
            InputManager.Instance.OnFire -= ShotPressed;
            InputManager.Instance.OnFireCanceled -= ShotReleased;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletArm = GetComponent<BulletArm>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        currentAnim = flyAnimations[0];
        // Forza la UI ad allinearsi SUBITO alla vera vita del giocatore (100) appena parte il livello
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateHealthSlider(health, maxHealth);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentAnim != null)
        {
            PlayerAnimation();
        }

    }

    private void FixedUpdate()
    {
        float speed = moveSpeed;
        rb.linearVelocity = moveInput * speed;
    }

    void PlayerAnimation()
    {
        string baseAnim = flyAnimations[0];
        if (moveInput.magnitude > 0.1f)
        {
            if (Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
            {
                baseAnim = moveInput.x > 0 ? flyAnimations[4] : flyAnimations[3];
            }
            else
            {
                baseAnim = moveInput.y > 0 ? flyAnimations[1] : flyAnimations[2];
            }
        }

        if (isShooting)
        {
            if (moveInput.x < -0.5f)
            {
                SetAnimation(currentAnim = shotAnimations[1], true);
            }
            else if(moveInput.x > 0.5f)
            {
                SetAnimation(currentAnim = shotAnimations[2], true);
            }
            else
            {
                SetAnimation(currentAnim = shotAnimations[0], true);
            }
        }
        else
        {
            SetAnimation(baseAnim, true);
        }
        
    }


    void SetAnimation(string anim, bool loop)
    {
        if (currentAnim == anim) return;
        skeletonAnimation.state.SetAnimation(0, anim, loop);
    }

    public void AddHealth(float amount)
    {
        // Se la vita è già al massimo, non raccogliere la cura e interrompi il codice
        if (health >= maxHealth) return;

        // Aggiungi i 10 punti della cura
        health += amount;

        // Se la vita supera 100 (es. 95 + 10 = 105), il Clamp la inchioda a 100
        health = Mathf.Clamp(health, 0f, maxHealth);

        // Invia il valore finale (100) alla UI
        if (UIController.Instance != null)
        {
            UIController.Instance.UpdateHealthSlider(health, maxHealth);
        }

        if (hitEffect != null)
        {
            hitEffect.FlashOnce(colorHealth, hitEffect.defaultDuration);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if(UIController.Instance != null)
        {
            UIController.Instance.UpdateHealthSlider(health, maxHealth);
        }

        if (health <= 0)
        {
            gameover = true;
            LevelUI.Instance.ShowGameOverPanel();
        }

        hitEffect.FlashOnce(colorDamage, hitEffect.defaultDuration);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("BulletEnemy"))
        {
            TakeDamage(collision.gameObject.GetComponent<Projectile2D>().damage);
        }
    }

    private void HandleMoveInput(Vector2 input) => moveInput = input;
    public bool HasMovementInput() => moveInput != Vector2.zero;

    public void ShotPressed()
    {
        isShooting = true;
    }
    public void ShotReleased() 
    {
        isShooting = false;
    }

    
}
