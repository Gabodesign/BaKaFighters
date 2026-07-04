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

    [Header("Sotto-Pannelli Opzioni (Area 70%)")]
    [SerializeField] private Toggle toggleAudio;
    [SerializeField] private Toggle toggleVideo;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private Button backSettingsPanel;
    private Color selectColor = new Color(1f, 0.6f, 0f, 1f);

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
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (quitPanel != null) quitPanel.SetActive(false);

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
    }
    public void CloseNewGamePopup()
    {
        newGamePopup.SetActive(false);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
        panelMainMenu.SetActive(false);
        audioSettingsPanel.SetActive(true);
        videoSettingsPanel.SetActive(false);

        // 1. Forza lo stato logico iniziale
        toggleAudio.isOn = true;
        toggleVideo.isOn = false;


        // 2. LA SOLUZIONE: Forziamo il Toggle ad entrare in Select Mode.
        // Usiamo una Coroutine per dare a Unity un millisecondo di tempo per attivare il pannello,
        // altrimenti il comando Select() fallirebbe perché l'oggetto si sta ancora svegliando.
        StartCoroutine(SelectDefaultToggle());
    }

    private IEnumerator SelectDefaultToggle()
    {
        // Aspetta la fine del frame corrente in modo che la UI sia totalmente visibile ed attiva
        yield return new WaitForEndOfFrame();

        if (toggleAudio != null)
        {
            toggleAudio.Select(); // Attiva visivamente lo stato Selected/SelectMode
        }
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        panelMainMenu.SetActive(true);
        audioSettingsPanel.SetActive(true);
        videoSettingsPanel.SetActive(false);
        toggleAudio.isOn = true;
        toggleVideo.isOn = false;
        
    }

    public void OpenQuit()
    {
        quitPanel.SetActive(true);
    }
    public void CloseQuit()
    {
        quitPanel.SetActive(false);
    }
    public void QuitGame()
    {
        Debug.Log("Chiusura del gioco in corso...");
        Application.Quit();
    }

    public void ShowAudioSettings()
    {
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(true);
        if (videoSettingsPanel != null) videoSettingsPanel.SetActive(false);
        Debug.Log("Visualizzazione Impostazioni Audio");
    }

    public void ShowVideoSettings()
    {
        if (audioSettingsPanel != null) audioSettingsPanel.SetActive(false);
        if (videoSettingsPanel != null) videoSettingsPanel.SetActive(true);
        Debug.Log("Visualizzazione Impostazioni Video");
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
