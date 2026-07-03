using UnityEngine;
using UnityEngine.UI;                  
using UnityEngine.SceneManagement;
using System.Collections;
public class MenuManager : MonoBehaviour
{
    [Header("Schermate Principali")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;

    [Header("Pannelli Secondari")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject quitPanel;



    [Header("Fade")]
    public Image fadeImage;                        
    public float fadeDuration = 0.3f;
    private bool isTransitioning = false;

    [Header("Scena di gioco")]
    public string loadingSceneName = "Level";

    private bool isGameStarted = false;
    private void Start()
    {
        // Setup iniziale della scena per sicurezza
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // Disattiviamo tutti i pannelli secondari
        if (newGamePopup != null) newGamePopup.SetActive(false);

        if (fadeImage != null)
        {
            Color colore = fadeImage.color;  
            colore.a = 0f;                   
            fadeImage.color = colore;        

            fadeImage.raycastTarget = false;  
            fadeImage.gameObject.SetActive(true); 
        }
    }

    public void OnLevelScene()
    {
        StartCoroutine(LoadSceneWithFade(loadingSceneName));
    }

    private void Update()
    {
        // Se il gioco non è ancora "iniziato" e il giocatore preme un tasto qualsiasi
        if (!isGameStarted && Input.anyKeyDown)
        {
            OpenMainMenu();
        }
    }

    // Passaggio dalla Start Screen al Menu Principale
    private void OpenMainMenu()
    {
        isGameStarted = true;
        startScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }

    public void OpenNewGamePopup()
    {
        newGamePopup.SetActive(true);
        panelMainMenu.SetActive(false);
    }
    public void CloseNewGamePopup()
    {
        newGamePopup.SetActive(false);
        panelMainMenu.SetActive(true);
    }

    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        isTransitioning = true;                 // Segniamo che è in corso una transizione.

        if (fadeImage == null)
        {
            // Se per qualche motivo non abbiamo un fadeImage, carichiamo direttamente.
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadeImage.raycastTarget = true;         // Blocchiamo input durante il fade.

        Color c = fadeImage.color;
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            float alpha = t / fadeDuration;     // 0 -> 1
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;           // Assicuriamo che sia completamente nero.
        fadeImage.color = c;

        // Carichiamo la scena (es. "Loading").
        SceneManager.LoadScene(sceneName);
    }
}
