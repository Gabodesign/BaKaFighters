using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class GameManager : MonoBehaviour
{

    public static GameManager Instance { get; private set; }

    [Header("Score")]
    [SerializeField] private TMP_Text scoreText;
    private int score = 0;
    private int highScore = 0;

    [Header("Fade")]
    public Image fadeImage;
    public float fadeDuration = 0.3f;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        scoreText.text = "Score: " + score;
        // 1. Congeliamo il tempo all'inizio
        Time.timeScale = 0;

        if (fadeImage != null)
        {
            // 2. Avviamo la transizione come Coroutine
            StartCoroutine(DoFadeFromBlack());
        }
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score.ToString("N0");
    }

    // Usiamo una Coroutine per spalmare il calcolo su più frame
    IEnumerator DoFadeFromBlack()
    {
        fadeImage.gameObject.SetActive(true);
        fadeImage.raycastTarget = true; // Blocca i click durante il fade

        float elapsedTime = 0f;
        Color colore = fadeImage.color;

        // Partiamo da Alpha 1
        colore.a = 1f;
        fadeImage.color = colore;

        while (elapsedTime < fadeDuration)
        {
            // Usiamo unscaledDeltaTime perché Time.timeScale è 0!
            elapsedTime += Time.unscaledDeltaTime;

            // Calcoliamo il progresso (da 0 a 1)
            float progress = elapsedTime / fadeDuration;

            // Invertiamo il progresso per andare da 1 a 0
            colore.a = Mathf.Clamp01(1f - progress);
            fadeImage.color = colore;

            // "Aspetta il prossimo frame" - Questo permette a Unity di renderizzare il cambiamento
            yield return null;
        }

        // Assicuriamoci che alla fine sia esattamente zero
        colore.a = 0f;
        fadeImage.color = colore;

        fadeImage.raycastTarget = false;
        // fadeImage.gameObject.SetActive(false); // Opzionale: disattiva l'oggetto

        // 3. Facciamo partire il tempo di gioco
        Time.timeScale = 1;
    }

    public void AddPoint(int amount)
    {
        score += amount;
        UpdateUI();

        if (score > highScore)
        {
            highScore = score;
        }
    }
    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void ResetScore()
    {
        score = 0;
    }

}
