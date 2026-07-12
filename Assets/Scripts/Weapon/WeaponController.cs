using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponController : MonoBehaviour
{
    [Header("Armi")]
    [SerializeField] public WeaponData[] weaponsData;
    [SerializeField] public WeaponType startingWeapon = WeaponType.Bullet;
    private WeaponType weaponEquip;
    public int currentWeaponLevel = 0;

    [Header("References")]
    public Transform armTransform; // L'osso Arm_A1 in Override


    [Header("Settings")]
    public float mouseSmoothSpeed = 15f;
    public float gamepadSmoothSpeed = 10f;

    // Il tuo offset magico basato sul file Spine
    private float offset = -167.95f;
    private float currentAngle;

    public Transform firePoint;
    public bool isShooting = false;

    // Variabile per memorizzare la direzione di puntamento corrente
    private Vector2 aimInput;

    private void Awake()
    {
        weaponEquip = startingWeapon;
    }

    private void OnEnable()
    {
        if (InputManager.Instance != null)
        {
            // Ci iscriviamo all'evento del puntamento
            InputManager.Instance.OnAim += HandleAimInput;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnAim -= HandleAimInput;
        }
    }

    private void HandleAimInput(Vector2 input)
    {
        aimInput = input;
    }

    void LateUpdate()
    {
        if (InputManager.Instance == null) return;

        float targetAngle = 0f;

        // Controlliamo lo schema di controllo corrente direttamente dall'InputManager
        string currentScheme = InputManager.Instance.controls.controlSchemes[0].name;
        // Nota: Se la stringa sopra ti dà problemi, puoi usare il controllo sulla periferica attiva:
        bool isGamepad = Gamepad.current != null && Gamepad.current.wasUpdatedThisFrame;

        if (isGamepad)
        {
            if (aimInput.sqrMagnitude > 0.1f)
            {
                targetAngle = Mathf.Atan2(aimInput.y, aimInput.x) * Mathf.Rad2Deg;
                targetAngle = Mathf.Clamp(targetAngle, -40f, 40f);
            }
            else
            {
                targetAngle = 0f;
            }
        }
        else
        {
            // Con il mouse, aimInput contiene la posizione dello schermo (Screen Position) XY
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 10f));
            Vector2 direction = mouseWorldPos - armTransform.position;

            targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            targetAngle = Mathf.Clamp(targetAngle, -40f, 40f);
        }

        // Applichiamo il calcolo dell'angolo e l'interpolazione fluida
        float finalTarget = targetAngle + offset;
        float lerpSpeed = isGamepad ? gamepadSmoothSpeed : mouseSmoothSpeed;

        currentAngle = Mathf.LerpAngle(currentAngle, finalTarget, Time.deltaTime * lerpSpeed);

        armTransform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }


    public void ShotPressed()
    {
        isShooting = true;
    }
    public void ShotReleased()
    {
        isShooting = false;
    }

    public void UpgradeWeapon()
    {
        if (currentWeaponLevel < weaponsData[(int)weaponEquip].levels.Length - 1)
        {
            currentWeaponLevel++;
            Debug.Log($"Weapon upgraded to level {currentWeaponLevel}");
        }
        else
        {
            Debug.Log("Weapon is already at max level.");
        }
    }
}