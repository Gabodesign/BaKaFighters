using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class WeaponController : MonoBehaviour
{

    [Header("References")]
    public Transform armTransform; // L'osso Arm_A1 in Override
    public PlayerInput playerInput; // Trascina qui il componente PlayerInput

    [Header("Settings")]
    public float mouseSmoothSpeed = 15f;
    public float gamepadSmoothSpeed = 10f;

    // Il tuo offset magico basato sul file Spine
    private float offset = -167.95f;
    private float currentAngle;

    void LateUpdate()
    {
        Vector2 aimInput = playerInput.actions["Aim"].ReadValue<Vector2>();
        float targetAngle = 0f;

        if (playerInput.currentControlScheme == "Gamepad")
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
            
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(aimInput.x, aimInput.y, 10f));
            Vector2 direction = mouseWorldPos - armTransform.position;

            targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            
            targetAngle = Mathf.Clamp(targetAngle, -40f, 40f);
        }

        
        float finalTarget = targetAngle + offset;
        float lerpSpeed = (playerInput.currentControlScheme == "Gamepad") ? gamepadSmoothSpeed : mouseSmoothSpeed;

        currentAngle = Mathf.LerpAngle(currentAngle, finalTarget, Time.deltaTime * lerpSpeed);

        armTransform.localRotation = Quaternion.Euler(0, 0, currentAngle);
    }
}
