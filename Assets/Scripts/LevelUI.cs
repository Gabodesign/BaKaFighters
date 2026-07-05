using System.Collections;
using TMPro;
using UnityEngine;

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

    private void Awake()
    {
        // Impostiamo l'istanza per la scena attuale
        Instance = this;
    }

    private void Start()
    {
        if (panelGameover != null) panelGameover.SetActive(false);

        // Chiediamo al GameManager il punteggio attuale per scriverlo all'avvio
        if (GameManager.Instance != null)
        {
            UpdateScoreUI(GameManager.Instance.GetScore());
        }

        if (canvasGroup != null)
        {
            StartCoroutine(FadeToTransparent());
        }
    }

    public void UpdateScoreUI(int currentScore)
    {
        if (scoreText != null)
            scoreText.text = "Score: " + currentScore.ToString("N0");
    }

    public void ShowGameOverPanel()
    {
        Time.timeScale = 0.1f;
        if (panelGameover != null)
            panelGameover.SetActive(true);
    }

    // Gestione dei pulsanti della UI che rimandano al GameManager
    public void OnClickRestart()
    {
        if (GameManager.Instance != null) GameManager.Instance.RestartLevel();
    }

    public void OnClickMainMenu()
    {
        if (GameManager.Instance != null) GameManager.Instance.ReturnMainMenu();
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
}