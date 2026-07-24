using UnityEngine;
using UnityEngine.UI;                  
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
public class MenuManager : MonoBehaviour
{
    [HideInInspector]
    public enum MenuState { Main, SaveLoad, NewGamePopup, LoadGamePopup, Options, Quit }
    public enum MainMenuSlotMode { NewGame, LoadGame }
    private MainMenuSlotMode currentSlotMode;


    [Header("Schermate Principali")]
    [SerializeField] private GameObject panelMainMenu;
    [SerializeField] private GameObject startScreenPanel;
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject loadgameButton;

    [Header("Pannelli Secondari")]
    [SerializeField] private GameObject newGamePopup;
    [SerializeField] private GameObject confirmNewGamePopup;
    [SerializeField] private GameObject overwriteNewGamePopup;
    [SerializeField] private GameObject saveLoadPanel;
    [SerializeField] private GameObject loadGamePopup;
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

    [Header("titoloSottopannello SaveLoad")]
    [SerializeField] private TextMeshProUGUI titlepage;

    [Header("titoli popup conferma NEW/LOAD game")]
    [SerializeField] private TextMeshProUGUI textNewGamePopUp;
    [SerializeField] private TextMeshProUGUI textLoadGamePopUp;

    [Header("Fade")]
    public Image fadeImage;                        
    public float fadeDuration = 0.3f;
    private bool isTransitioning = false;

    [Header("Save Slots Panel")]
    [SerializeField] private SaveSlotsPanel saveSlotsPanel;

    [Header("Scena di gioco")]
    private int selectedSlotIndex = -1; // Memorizza quale slot (0, 1 o 2) è stato cliccato
    private bool isGameStarted = false;
    private void Start()
    {
        // Setup iniziale della scena per sicurezza
        if (panelMainMenu != null) panelMainMenu.SetActive(true);
        if (startScreenPanel != null) startScreenPanel.SetActive(true);
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);

        // Disattiviamo tutti i pannelli secondari
        if (newGamePopup != null) newGamePopup.SetActive(false);
        if (loadGamePopup != null) loadGamePopup.SetActive(false); 
        if (saveLoadPanel != null) saveLoadPanel.SetActive(false);
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

    private void OnEnable()
    {
        if(InputManager.Instance != null)
        {
            InputManager.Instance.OnCanceled += CloseSaveLoad;
        }
    }

    private void OnDisable()
    {
        if (InputManager.Instance != null)
        {
            InputManager.Instance.OnCanceled -= CloseSaveLoad;
        }
    }

    // Passaggio dalla Start Screen al Menu Principale
    private void OpenMainMenu()
    {
        isGameStarted = true;
        startScreenPanel.SetActive(false);
        mainMenuPanel.SetActive(true);

        UpdateLoadButtonVisibility(); // Aggiorna la visibilità del pulsante LoadGame
    }

    //funzione per aggiornare la visibilità del pulsante LoadGame in base alla presenza di salvataggi
    public void UpdateLoadButtonVisibility()
    {
        if (loadgameButton != null && GameManager.Instance != null)
        {
            // Se HasAnySaveFile() è false, SetActive riceve false e NASCONDE il pulsante
            loadgameButton.SetActive(GameManager.Instance.HasAnySaveFile());
        }
    }

    // Collega questo al pulsante "NUOVA PARTITA" del Menu Principale
    public void OpenForNewGame()
    {
        currentSlotMode = MainMenuSlotMode.NewGame;
        if (titlepage != null) titlepage.text = "NUOVA PARTITA - SELEZIONA SLOT";
        if (saveSlotsPanel != null) saveSlotsPanel.Populate(currentSlotMode);
        OpenSaveLoad();
    }

    // Collega questo al pulsante "CARICA PARTITA" del Menu Principale
    public void OpenForLoadGame()
    {
        currentSlotMode = MainMenuSlotMode.LoadGame;
        if (titlepage != null) titlepage.text = "SELEZIONA PARTITA DA CARICARE";
        if (saveSlotsPanel != null) saveSlotsPanel.Populate(currentSlotMode);
        OpenSaveLoad();
    }


    // --- METODI PER I 3 SLOT ---

    // Collega questo metodo agli OnClick dei 3 Slot nell'Hierarchy (passando 0, 1 o 2)
    public void OnSelectSlot(int slotIndex)
    {

        Debug.Log($"Modalità: {currentSlotMode}");

        selectedSlotIndex = slotIndex;
        bool hasData = GameManager.Instance.saveSlots[slotIndex] != null;

        if (currentSlotMode == MainMenuSlotMode.NewGame)
        {
            if (hasData)
            {
                // Slot occupato: Chiediamo conferma per sovrascrivere
                if (textNewGamePopUp != null) textNewGamePopUp.text = $"Lo Slot {slotIndex + 1} contiene già una partita.\nVuoi sovrascriverla?";
                OpenNewGamePopup();
                if (confirmNewGamePopup != null) confirmNewGamePopup.SetActive(false);
                if (overwriteNewGamePopup != null) overwriteNewGamePopup.SetActive(true);
            }
            else
            {
                // Slot vuoto: Iniziamo subito la Nuova Partita
                if (textNewGamePopUp != null) textNewGamePopUp.text = $"Lo Slot {slotIndex + 1} vuoto.\nVuoi iniziare una nuova partita?";
                OpenNewGamePopup();
                if (confirmNewGamePopup != null) confirmNewGamePopup.SetActive(true);
                if (overwriteNewGamePopup != null) overwriteNewGamePopup.SetActive(false);
            }
        }
        else if (currentSlotMode == MainMenuSlotMode.LoadGame)
        {
            if (hasData)
            {
                if (textLoadGamePopUp != null) textLoadGamePopUp.text = $"Vuoi caricare la partita dello Slot {slotIndex + 1}?";
                OpenLoadGamePopup();
            }
        }
    }

    // Collega questo metodo al tasto "SÌ" del PopUp di Caricamento/Conferma
    public void ConfirmLoadGame()
    {
        if (selectedSlotIndex >= 0)
        {

            if (currentSlotMode == MainMenuSlotMode.NewGame)
            {
                // Se stiamo creando una Nuova Partita su uno slot occupato, cancelliamo i vecchi dati
                GameManager.Instance.DeleteGame(selectedSlotIndex);
                StartNewGameOnSelectedSlot();
            }
            else if (currentSlotMode == MainMenuSlotMode.LoadGame)
            {
                // Carichiamo i dati dello slot selezionato
                GameManager.Instance.LoadGame(selectedSlotIndex);
            }
        }
    }

    // Collega questo metodo all'eventuale tasto "ELIMINA" o "CESTINO" dello slot
    public void ConfirmDeleteSlot(int slotIndex)
    {
        GameManager.Instance.DeleteGame(slotIndex);
        UpdateLoadButtonVisibility();
        // Qui puoi rinfrescare graficamente lo slot a schermo per mostrare "Vuoto"
        if (saveSlotsPanel != null) saveSlotsPanel.Populate(currentSlotMode);
    }

    public void StartNewGameOnSelectedSlot()
    {
        GameManager.Instance.currentSlot = selectedSlotIndex;
        GameManager.Instance.SaveGame(selectedSlotIndex);
        // Resettiamo eventuali dati temporanei e carichiamo il primo livello
        StartCoroutine(LoadSceneWithFade("TestLevel"));
    }




    // Funzioni per aprire e chiudere i pannelli NewGame da aprire quando clicchiamo uno slot
    public void OpenNewGamePopup()
    {
        SetState(MenuState.NewGamePopup);
    }


    public void CloseNewGamePopup()
    {
        SetState(MenuState.SaveLoad);
        currentSlotMode = MainMenuSlotMode.NewGame;
    }


    // Funzioni per aprire e chiudere i pannelli SaveLoad
    public void OpenSaveLoad()
    {
        SetState(MenuState.SaveLoad);
    }

    public void CloseSaveLoad()
    {
        SetState(MenuState.Main);
    }

    // Funzioni per aprire e chiudere i pannelli LoadGamePopup qundo clicchiamo uno slot

    public void OpenLoadGamePopup()
    {
        SetState(MenuState.LoadGamePopup);
    }

    public void CloseLoadGamePopup()
    {
        SetState(MenuState.SaveLoad);
        currentSlotMode = MainMenuSlotMode.LoadGame;
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
        saveLoadPanel.SetActive(state == MenuState.SaveLoad);
        newGamePopup.SetActive(state == MenuState.NewGamePopup);
        loadGamePopup.SetActive(state == MenuState.LoadGamePopup);
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
