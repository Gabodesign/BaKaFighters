using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(1000)]
public class WeaponController : MonoBehaviour
{
    [Header("Armi")]
    [SerializeField] public WeaponData[] weaponsData;
    [SerializeField] public WeaponType startingWeapon = WeaponType.Bullet;
    private WeaponType weaponEquip;
    public int currentWeaponLevel = 0;

    [Header("References")]
    public Transform armTransform; // L'osso Arm_A1
    public Transform firePoint;
    [Header("Settings - Fluidità Puntatore")]
    public float smoothSpeed = 30f;

    [Header("Settings - Limiti di Rotazione")]
    [SerializeField] private float offset = -167.95f;

    [Tooltip("Limite inferiore: quanto può scendere l'arma (es. -120, -150 per andare verso il basso)")]
    [SerializeField] private float minAngle = -130f;

    [Tooltip("Limite superiore: quanto può salire l'arma (es. 40, 60 per andare verso l'alto)")]
    [SerializeField] private float maxAngle = 20f;

    public bool isShooting = false;

    private float currentAngle;
    private Vector2 aimInput;
    private bool isGamepad = false;
   
    public WeaponType WeaponEquip => weaponEquip;

    private void Awake()
    {
        weaponEquip = startingWeapon;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAim += HandleAimInput;
        }
        InputSystem.onActionChange += OnActionChange;
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAim -= HandleAimInput;
        }
        InputSystem.onActionChange -= OnActionChange;
    }

    private void OnActionChange(object obj, InputActionChange change)
    {
        if (change == InputActionChange.ActionPerformed && obj is InputAction action)
        {
            if (action.activeControl != null)
            {
                isGamepad = action.activeControl.device is Gamepad;
            }
        }
    }

    private void HandleAimInput(Vector2 input)
    {
        aimInput = input;
    }

    private void LateUpdate()
    {
        if (armTransform == null || Camera.main == null) return;

        float targetAngle = 0f;

        if (isGamepad)
        {
            if (aimInput.sqrMagnitude > 0.08f)
            {
                float rawGamepadAngle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;

                // Applicazione dei limiti prima dell'offset
                float clampedGamepadAngle = Mathf.Clamp(rawGamepadAngle, minAngle, maxAngle);
                targetAngle = clampedGamepadAngle + offset;
            }
            else return;
        }
        else
        {
            // 1. Convertiamo il mouse nello spazio Mondo
            Vector3 mouseScreenPos = new Vector3(aimInput.x, aimInput.y, Mathf.Abs(Camera.main.transform.position.z - armTransform.position.z));
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            // 2. Vettore direzione dall'osso al mouse
            Vector2 dir = mouseWorldPos - armTransform.position;

            // 3. Calcolo dell'angolo puro della mira
            float rawAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // 4. CLAMP: Limitiamo l'angolo puro tra minAngle e maxAngle
            float clampedAngle = Mathf.Clamp(rawAngle, minAngle, maxAngle);

            // 5. Sommiamo l'offset di Spine SOLO alla fine
            targetAngle = clampedAngle + offset;
        }

        // Gestione personaggio girato a sinistra
        if (transform.lossyScale.x < 0)
        {
            targetAngle = 180f - targetAngle;
        }

        // Interpolazione fluida della rotazione
        if (smoothSpeed > 0)
        {
            currentAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * smoothSpeed);
        }
        else
        {
            currentAngle = targetAngle;
        }

        armTransform.localEulerAngles = new Vector3(0, 0, currentAngle);
    }

    public void ShotPressed() => isShooting = true;
    public void ShotReleased() => isShooting = false;

    public void UpgradeWeapon()
    {
        if (currentWeaponLevel < weaponsData[(int)weaponEquip].levels.Length - 1)
        {
            currentWeaponLevel++;
            Debug.Log($"Weapon upgraded to level {currentWeaponLevel}");
        }
    }
}