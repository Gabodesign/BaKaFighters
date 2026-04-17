using Spine.Unity;
using UnityEngine;
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
    private PlayerControls controls;             
    private Vector2 moveInput;                   
    public bool isShooting = false;

    [Header("Component Player Health")]   
    [SerializeField] public float health;    
    [SerializeField] private float maxHealth; 

    [Header("Component Player Shield")]   
    [SerializeField] public float shield;    
    [SerializeField] private float maxShield; 

    [Header("Component Player Ki")]       
    [SerializeField] public float ki;        
    [SerializeField] private float maxKi;

    [Header("Armi")]
    [SerializeField] private WeaponsConfig[] weaponConfigs;
    [SerializeField] private WeaponType startingWeapon = WeaponType.Bullet;

    private BulletArm bulletArm;

    public Transform firePoint;
    private Rigidbody2D rb;

    private void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        controls = new PlayerControls();
        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;
        controls.Player.Fire.performed += ctx => ShotPressed();  
        controls.Player.Fire.canceled += ctx => ShotReleased(); 
    }

    void OnEnable()
    { 
        if (controls != null)
        {
            controls.Enable();
        }
    }
    
    void OnDisable()
    {   
        if (controls != null)
        {
            controls.Disable();
        }   
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        bulletArm = GetComponent<BulletArm>();
        rb = GetComponent<Rigidbody2D>();
        health = maxHealth;
        currentAnim = flyAnimations[0];

    }

    // Update is called once per frame
    void Update()
    {
        if(currentAnim != null)
        {
            PlayerAnimation();
        }
        UIController.Instance.UpdateHealthSlider(health, maxHealth);
        UIController.Instance.UpdateShieldSlider(shield, maxShield);
        UIController.Instance.UpdateKiSlider(ki, maxKi);

    }

    private void FixedUpdate()
    {
        float speed = moveSpeed;
        rb.linearVelocity = moveInput * speed;
    }

    void PlayerAnimation()
    {
        
        if(Mathf.Abs(moveInput.x) > Mathf.Abs(moveInput.y))
        {
            if(moveInput.x < 0)
            {
                SetAnimation(currentAnim = flyAnimations[3], true);
            }
            else if(moveInput.x > 0)
            {
                SetAnimation(currentAnim = flyAnimations[4], true);
            }
        }
        else
        {
            if (moveInput.y < 0)
            {
                SetAnimation(currentAnim = flyAnimations[2], true);
            }
            else if(moveInput.y > 0)
            {
                SetAnimation(currentAnim = flyAnimations[1], true);
            }
        }

        if (isShooting)
        {
            if (moveInput.x < 0)
            {
                SetAnimation(currentAnim = shotAnimations[1], true);
            }
            else if(moveInput.x > 0)
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
            SetAnimation(currentAnim, true);
        }
    }


    void SetAnimation(string anim, bool loop)
    {
        skeletonAnimation.state.SetAnimation(0, anim, loop);
    }

    public void AddHealth(float amount)
    {
        float difference = maxHealth - health; //100 - 95 = 5

        if (health < maxHealth)
        {
            if (difference > 9)
            {
                health += amount;
            }
            else
            {
                health += difference;
            }
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void ShotPressed()
    {
        isShooting = true;
    }
    public void ShotReleased() 
    {
        isShooting = false;
    }
}
