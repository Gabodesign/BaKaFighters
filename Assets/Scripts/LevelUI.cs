using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [Header("Menu Pausa")]
    [SerializeField] public GameObject panelPausa;
    [Header("Timer")]
    [SerializeField] private TMP_Text timerText;
    [Header("Lives")]
    [SerializeField] private TMP_Text livesText;
    private float timer;
    private void Awake()
    {
        // Impostiamo l'istanza per la scena attuale
        Instance = this;
    }

    private void Start()
    {
        if (panelGameover != null) panelGameover.SetActive(false);
        if (panelPausa != null) panelPausa.SetActive(false);

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
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null && InputManager.Instance.controls != null)
        {
            InputManager.Instance.OnPause -= ShowPausa;
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
    }

    // Gestione dei pulsanti della UI che rimandano al GameManager
    public void OnClickRestart()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.playerLives = 3;
            GameManager.Instance.RestartLevel();
        }
    }

    public void OnClickMainMenu()
    {
        if (GameManager.Instance != null){
            GameManager.Instance.playerLives = 3;
            GameManager.Instance.ReturnMainMenu();
        }
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
        float totalTime = Time.timeSinceLevelLoad;

        int minutes = (int)(totalTime / 60f) % 60;
        int seconds = (int)(totalTime % 60f);
        int milliseconds = (int)(totalTime * 1000f) % 1000;

        timerText.text = "TIME: " + minutes.ToString("D2") + ":" + seconds.ToString("D2") + ":" + milliseconds.ToString("D2");
    }


    public void ShowPausa()
    {
        if (panelPausa == null) return;

        // Controlliamo se il pannello della pausa è attualmente ATTIVO nella scena
        bool isPaused = panelPausa.activeSelf;

        if (!isPaused)
        {
            // SE NON ERA IN PAUSA: Congeliamo il gioco e attiviamo il menu
            AudioManager.instance.PlaySFX(AudioManager.instance.pause);
            Time.timeScale = 0f;
            panelPausa.SetActive(true);
        }
        else
        {
            // SE ERA GIÀ IN PAUSA: Facciamo ripartire il tempo e nascondiamo il menu
            AudioManager.instance.PlaySFX(AudioManager.instance.unpause);
            Time.timeScale = 1f;
            panelPausa.SetActive(false);
        }
    }

}