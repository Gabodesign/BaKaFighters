using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-100)]
public class InputManager : MonoBehaviour
{

    public static InputManager Instance;
    public PlayerControls controls;
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
        controls.Enable();
        controls.Player.Move.performed += ctx => OnMove?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Move.canceled += ctx => OnMove?.Invoke(Vector2.zero);

        controls.Player.Aim.performed += ctx => OnAim?.Invoke(ctx.ReadValue<Vector2>());
        controls.Player.Aim.canceled += ctx => OnAim?.Invoke(Vector2.zero);

        controls.Player.Fire.performed += ctx => OnFire?.Invoke();
        controls.Player.Fire.canceled += ctx => OnFireCanceled?.Invoke();

        controls.Player.Pause.performed += ctx => OnPause?.Invoke();

        controls.UI.Cancel.performed += ctx => OnCanceled?.Invoke();
        controls.UI.DeleteSave.performed += ctx => OnDeleteSave?.Invoke();

    }

    private void OnDisable()
    {
        if (controls != null)
        {
            controls.Disable();
        }
    }


    public event System.Action<Vector2> OnMove;
    public event System.Action<Vector2> OnAim;
    public event System.Action OnFire;
    public event System.Action OnFireCanceled;
    public event System.Action OnPause;
    public event System.Action OnCanceled;
    public event System.Action OnDeleteSave;
}
