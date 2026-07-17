using UnityEngine;
using UnityEngine.UI;                  
using UnityEngine.SceneManagement;
using System.Collections;
public class MenuManager : MonoBehaviour
{

    [HideInInspector]
    public enum MenuState { Main, NewGamePopup, Options, Quit }

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
    [SerializeField] private Toggle toggleComands;
    [SerializeField] private Toggle toggleResults;
    [SerializeField] private GameObject audioSettingsPanel;
    [SerializeField] private GameObject videoSettingsPanel;
    [SerializeField] private GameObject comandsSettingsPanel;
    [SerializeField] private GameObject resultsSettingsPanel;
    [SerializeField] private Button backSettingsPanel;
    private Color selectColor = new Color(1f, 0.6f, 0f, 1f);

    [Header("Fade")]
    public Image fadeImage;                        
    public float fadeDuration = 0.3f;
    private bool isTransitioning = false;

    [Header("Scena di gioco")]

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
        SetState(MenuState.NewGamePopup);
    }
    public void CloseNewGamePopup()
    {
        SetState(MenuState.Main);
    }

    public void OpenOptions()
    {
        SetState(MenuState.Options);
    
        // 1. Forza lo stato logico iniziale
        toggleAudio.isOn = true;
        toggleVideo.isOn = false;
        toggleComands.isOn = false;
        toggleResults.isOn = false;


        // 2. LA SOLUZIONE: Forziamo il Toggle ad entrare in Select Mode.

        StartCoroutine(SelectDefaultToggle());
    }


    public void CloseOptions()
    {
        SetState(MenuState.Main);
        audioSettingsPanel.SetActive(true);
        videoSettingsPanel.SetActive(false);
        comandsSettingsPanel.SetActive(false);
        resultsSettingsPanel.SetActive(false);
        toggleAudio.isOn = true;
        toggleVideo.isOn = false;
        toggleComands.isOn = false;
        toggleResults.isOn = false;
    }

    public void OpenQuit()
    {
        SetState(MenuState.Quit);
    }
    public void CloseQuit()
    {
        SetState(MenuState.Main);
    }
    public void QuitGame()
    {
        Debug.Log("Chiusura del gioco in corso...");
        Application.Quit();
    }



    public void SetState(MenuState state)
    {
        panelMainMenu.SetActive(state == MenuState.Main);
        newGamePopup.SetActive(state == MenuState.NewGamePopup);
        optionsPanel.SetActive(state == MenuState.Options);
        quitPanel.SetActive(state == MenuState.Quit);
    }

    public void OnLevelScene()
    {
        StartCoroutine(LoadSceneWithFade("TestLevel"));
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
