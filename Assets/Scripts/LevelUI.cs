using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class LevelUI : MonoBehaviour
{
    // Istanza locale valida solo ed esclusivamente per questa scena corrente
    public static LevelUI Instance { get; private set; }

    [Header("Score UI")]
    [SerializeField] private TMP_Text scoreText;

    [Header("Fade UI")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 0.3f;

    [Header("GameOver UI")]
    [SerializeField] private GameObject panelGameover;
    [SerializeField] private Button gameOverFirstButton;
    [Header("Menu Pausa")]
    [SerializeField] public GameObject panelPausa;
    [Header("Level")]
    [SerializeField] private GameObject level;
    [Header("Button Save/Load")]
    [SerializeField] public Button saveButton;
    [SerializeField] public Button loadButton;
    [Header("Carica da pausa (opzionale: pannello slot)")]
    [SerializeField] private SaveSlotsPanel pauseLoadPanel;

    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    private float crono;
  
    [Header("Lives")]
    [SerializeField] private TMP_Text livesText;

    public GameObject Level => level;
    public float Crono => crono;

    private void Awake()
    {
        // Impostiamo l'istanza per la scena attuale
        Instance = this;
        // Il timer riparte da quello che decide il GameManager (0 per nuova partita/riprova, il valore salvato se carichiamo)
        crono = GameManager.Instance != null ? GameManager.Instance.StartingTimer : 0f;

        // Se stiamo caricando una partita, il livello riparte dalla posizione salvata (prima di tutti gli Start)
        if (GameManager.Instance != null && GameManager.Instance.TryGetLoadedState(out SaveData loaded) && level != null)
        {
            level.transform.position = loaded.PosLevel;
        }
    }

    private void Start()
    {
        if (panelGameover != null) panelGameover.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        if (saveButton != null)
        {
            saveButton.onClick.RemoveAllListeners();
            saveButton.onClick.AddListener(() => GameManager.Instance.SaveGamePausa());
        }

        if (loadButton != null)
        {
            loadButton.onClick.RemoveAllListeners();
            loadButton.onClick.AddListener(OpenLoadFromPausa);
        }
        // Riprova e Torna al menu del Game Over sono collegati dall'Inspector (OnClickRestart / OnClickMainMenu):
        // un solo modo per pulsante, altrimenti l'azione verrebbe eseguita due volte.
        if (pauseLoadPanel != null) pauseLoadPanel.gameObject.SetActive(false);

        // Chiediamo al GameManager il punteggio attuale per scriverlo all'avvio
        if (GameManager.Instance != null)
        {
            UpdateLivesUI(GameManager.Instance.playerLives);
            UpdateScoreUI(GameManager.Instance.GetScore());
        }

        if (canvasGroup != null)
        {
            StartCoroutine(FadeToTransparent());
        }

    }
    private void OnEnable()
    {
        // Il LevelUI controlla se l'InputManager è pronto e si iscrive da solo!
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            InputManager.Instance.OnPause -= ShowPausa; // Pulizia preventiva
            InputManager.Instance.OnPause += ShowPausa; // Iscrizione sicura al 100%
            InputManager.Instance.OnCanceled -= HandleCancel;
            InputManager.Instance.OnCanceled += HandleCancel;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            InputManager.Instance.OnPause -= ShowPausa;
            InputManager.Instance.OnCanceled -= HandleCancel;
        }
    }
    public void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString("N0");
    }

    public void UpdateLivesUI(int currentLives)
    {
        if (livesText != null)
            livesText.text = "Lives: " + currentLives.ToString("N0");
    }

    public void ShowGameOverPanel()
    {
        Time.timeScale = 0f;
        if (panelGameover != null)
            panelGameover.SetActive(true);
        if (InputManager.Instance != null)
            InputManager.Instance.EnableUI();
        if (gameOverFirstButton != null)
        {
            StartCoroutine(SelectFirstGameOverButton());
        }
    }

    private IEnumerator SelectFirstGameOverButton()
    {
        yield return null;
        if (EventSystem.current == null) yield break;

        Button first = gameOverFirstButton != null ? gameOverFirstButton : panelGameover.GetComponentInChildren<Button>();
        if (first == null) yield break;

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(first.gameObject);
    }

    // Gestione dei pulsanti della UI che rimandano al GameManager
    public void OnClickRestart()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartLevel();
    }

    public void OnClickMainMenu()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ReturnMainMenu();
    }


    // --- COROUTINE DEL FADE ---


    private IEnumerator FadeToTransparent()
    {
        if (canvasGroup == null) yield break;

        float time = 0;
        float startAlpha = canvasGroup.alpha = 1f; // Parte da nero/coprente
        float endAlpha = 0f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
    }

    // Questo metodo permette al GameManager di eseguire il fade out e poi eseguire un'azione (caricare la scena)
    public void StartFadeOut(System.Action onFadeComplete)
    {
        if (canvasGroup != null)
        {
            StartCoroutine(FadeToBlack(onFadeComplete));
        }
        else
        {
            onFadeComplete?.Invoke();
        }
    }

    private IEnumerator FadeToBlack(System.Action onFadeComplete)
    {
        float time = 0;
        float startAlpha = canvasGroup.alpha;
        float endAlpha = 1f;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = endAlpha;

        // Eseguiamo il caricamento della scena passato come parametro
        onFadeComplete?.Invoke();
    }

    public void IncreaseTime()
    {
        crono += Time.deltaTime;

        int minutes = (int)(crono / 60f) % 60;
        int seconds = (int)(crono % 60f);
        int centiseconds = (int)(crono * 100f) % 100;

        timerText.text = "TIME: " + minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + centiseconds.ToString("D2");
    }
    public void ResetTime()
    {
        crono = 0f;
    }
    public void RestoreTime(float time)
    {
        crono = time;
    }

    private int lastPauseToggleFrame = -1;


    public void ShowPausa()
    {
        if (panelPausa == null) return;
        // Esc è sia "Pause" (mappa Player) sia "Cancel" (mappa UI): evitiamo che lo stesso tasto apra e chiuda nello stesso frame
        if (lastPauseToggleFrame == Time.frameCount) return;
        lastPauseToggleFrame = Time.frameCount;

        // Controlliamo se il pannello della pausa è attualmente ATTIVO nella scena
        bool isPaused = panelPausa.activeSelf;

        if (!isPaused)
        {
            InputManager.Instance.EnableUI();
            // SE NON ERA IN PAUSA: Congeliamo il gioco e attiviamo il menu
            AudioManager.instance.PlaySFX(AudioManager.instance.pause);
            Time.timeScale = 0f;
            panelPausa.SetActive(true);
        }
        else
        {
            // SE ERA GIÀ IN PAUSA: Facciamo ripartire il tempo e nascondiamo il menu
            AudioManager.instance.PlaySFX(AudioManager.instance.unpause);
            ClosePausa();
        }
    }

    /// <summary>Chiude il menu di pausa e fa ripartire il gioco.</summary>
    public void ClosePausa()
    {
        lastPauseToggleFrame = Time.frameCount;
        if (pauseLoadPanel != null) pauseLoadPanel.gameObject.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);
        Time.timeScale = 1f;
        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();
    }

    // Esc (Cancel della mappa UI): prima chiude la scelta dello slot, poi la pausa
    private void HandleCancel()
    {
        if (panelPausa == null || !panelPausa.activeSelf) return;

        if (pauseLoadPanel != null && pauseLoadPanel.gameObject.activeSelf)
        {
            pauseLoadPanel.gameObject.SetActive(false);
            if (loadButton != null && EventSystem.current != null)
                EventSystem.current.SetSelectedGameObject(loadButton.gameObject);
            return;
        }
        ShowPausa();
    }

    // Carica dalla pausa: se c'è il pannello slot si sceglie il salvataggio, altrimenti si ricarica lo slot in uso
    private void OpenLoadFromPausa()
    {
        if (pauseLoadPanel == null)
        {
            GameManager.Instance.LoadGamePausa();
            return;
        }
        pauseLoadPanel.gameObject.SetActive(true);
        pauseLoadPanel.Populate(MenuManager.MainMenuSlotMode.LoadGame, slot => GameManager.Instance.LoadGame(slot));
    }

}