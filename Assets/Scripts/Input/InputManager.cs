using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour,
    PlayerControls.IPlayerActions,
    PlayerControls.IUIActions
{
    public static InputManager Instance { get; private set; }
    public PlayerControls controls;

    public enum TestInputScheme { All, KeyboardMouseOnly, GamepadOnly }

    [Header("Debug - Test Input Scheme")]
    [SerializeField] private TestInputScheme testScheme = TestInputScheme.All;

    // --- Eventi pubblici: invariati, nessun altro script va modificato ---
    public event System.Action<Vector2> OnMove;
    public event System.Action<Vector2> OnAim;
    public event System.Action OnFire;
    public event System.Action OnFireCanceled;
    public event System.Action OnPause;
    public event System.Action OnCanceled;
    public event System.Action OnDeleteSave;

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
    }

    private void OnEnable()
    {
        // Il duplicato distrutto in Awake non ha creato "controls", ma OnEnable viene chiamato comunque
        if (controls == null) return;

        controls.Enable();
        ApplyBindingMask();

        controls.Player.AddCallbacks(this);
        controls.UI.AddCallbacks(this);
    }

    private void OnDisable()
    {
        if (controls == null) return;

        controls.Player.RemoveCallbacks(this);
        controls.UI.RemoveCallbacks(this);
        controls.Disable();
    }

    private void ApplyBindingMask()
    {
        switch (testScheme)
        {
            case TestInputScheme.KeyboardMouseOnly:
                controls.bindingMask = InputBinding.MaskByGroup("Keyboard&Mouse");
                break;
            case TestInputScheme.GamepadOnly:
                controls.bindingMask = InputBinding.MaskByGroup("Gamepad");
                break;
            default:
                controls.bindingMask = null; // nessuna restrizione
                break;
        }
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

    // =====================================================================
    // MAPPA PLAYER
    // Implementazione esplicita: serve perché l'interfaccia richiede metodi
    // chiamati OnMove, OnAim... che andrebbero in conflitto con gli eventi
    // pubblici omonimi.
    // =====================================================================

    void PlayerControls.IPlayerActions.OnMove(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return; // started e performed arrivano insieme: evita il doppio evento
        OnMove?.Invoke(ctx.ReadValue<Vector2>()); // su canceled il valore è già zero
    }

    void PlayerControls.IPlayerActions.OnAim(InputAction.CallbackContext ctx)
    {
        if (ctx.started) return;
        OnAim?.Invoke(ctx.ReadValue<Vector2>());
    }

    void PlayerControls.IPlayerActions.OnFire(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) OnFire?.Invoke();
        else if (ctx.canceled) OnFireCanceled?.Invoke();
    }

    void PlayerControls.IPlayerActions.OnPause(InputAction.CallbackContext ctx)
    {
        // Senza il filtro la pausa si attiverebbe e disattiverebbe nello stesso frame
        if (ctx.performed) OnPause?.Invoke();
    }

    // =====================================================================
    // MAPPA UI
    // =====================================================================

    void PlayerControls.IUIActions.OnCancel(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) OnCanceled?.Invoke();
    }

    void PlayerControls.IUIActions.OnDeleteSave(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) OnDeleteSave?.Invoke();
    }

    // Azioni standard gestite dall'EventSystem: metodi volutamente vuoti.
    // Se ne mancano o ne hai di più nel tuo asset, il compilatore te lo segnala.
    void PlayerControls.IUIActions.OnNavigate(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnSubmit(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnPoint(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnClick(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnRightClick(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnMiddleClick(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnScrollWheel(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnTrackedDevicePosition(InputAction.CallbackContext ctx) { }
    void PlayerControls.IUIActions.OnTrackedDeviceOrientation(InputAction.CallbackContext ctx) { }
}