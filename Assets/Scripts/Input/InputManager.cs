using System;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public PlayerControls controls;

    public enum TestInputScheme { All, KeyboardMouseOnly, GamepadOnly }

    [Header("Debug - Test Input Scheme")]
    [SerializeField] private TestInputScheme testScheme = TestInputScheme.All;

    private void ApplyBindingMask()
    {
        // seleziono il tipo di input 
        switch (testScheme)
        {
            case TestInputScheme.KeyboardMouseOnly:
                controls.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse"); // solo tastiera e mouse
                break;
            case TestInputScheme.GamepadOnly:
                controls.bindingMask = InputBinding.MaskByGroup("Gamepad"); // solo gamepad
                break;
            default:
                controls.bindingMask = null; // nessuna restrizione, tutto attivo
                break;
        }
    }

    private Action<InputAction.CallbackContext> onMovePerformed;
    private Action<InputAction.CallbackContext> onMoveCanceled;
    private Action<InputAction.CallbackContext> onAimPerformed;
    private Action<InputAction.CallbackContext> onAimCanceled;
    private Action<InputAction.CallbackContext> onFirePerformed;
    private Action<InputAction.CallbackContext> onFireCanceled;
    private Action<InputAction.CallbackContext> onPausePerformed;
    private Action<InputAction.CallbackContext> onCancelPerformed;
    private Action<InputAction.CallbackContext> onDeleteSavePerformed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        controls = new PlayerControls();

        // Le lambda vengono create UNA SOLA VOLTA qui, non ad ogni OnEnable
        onMovePerformed = ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        onMoveCanceled = ctx => OnMove?.Invoke(Vector2.zero);
        onAimPerformed = ctx => OnAim?.Invoke(ctx.ReadValue<Vector2>());
        onAimCanceled = ctx => OnAim?.Invoke(Vector2.zero);
        onFirePerformed = ctx => OnFire?.Invoke();
        onFireCanceled = ctx => OnFireCanceled?.Invoke();
        onPausePerformed = ctx => OnPause?.Invoke();
        onCancelPerformed = ctx => OnCanceled?.Invoke();
        onDeleteSavePerformed = ctx => OnDeleteSave?.Invoke();
    }

    private void OnEnable()
    {
        controls.Enable();
        ApplyBindingMask();

        controls.Player.Move.performed += onMovePerformed;
        controls.Player.Move.canceled += onMoveCanceled;

        controls.Player.Aim.performed += onAimPerformed;
        controls.Player.Aim.canceled += onAimCanceled;

        controls.Player.Fire.performed += onFirePerformed;
        controls.Player.Fire.canceled += onFireCanceled;

        controls.Player.Pause.performed += onPausePerformed;

        controls.UI.Cancel.performed += onCancelPerformed;
        controls.UI.DeleteSave.performed += onDeleteSavePerformed;
    }

    private void OnDisable()
    {
        if (controls == null) return;

        controls.Player.Move.performed -= onMovePerformed;
        controls.Player.Move.canceled -= onMoveCanceled;

        controls.Player.Aim.performed -= onAimPerformed;
        controls.Player.Aim.canceled -= onAimCanceled;

        controls.Player.Fire.performed -= onFirePerformed;
        controls.Player.Fire.canceled -= onFireCanceled;

        controls.Player.Pause.performed -= onPausePerformed;

        controls.UI.Cancel.performed -= onCancelPerformed;
        controls.UI.DeleteSave.performed -= onDeleteSavePerformed;
        controls.Disable();
    }

    public void EnableGameplay()
    {
        controls.Player.Enable();
        controls.UI.Disable();
    }

    public void EnableUI()
    {
        controls.Player.Disable();
        controls.UI.Enable();
    }

    public event System.Action<Vector2> OnMove;
    public event System.Action<Vector2> OnAim;
    public event System.Action OnFire;
    public event System.Action OnFireCanceled;
    public event System.Action OnPause;
    public event System.Action OnCanceled;
    public event System.Action OnDeleteSave;
}

