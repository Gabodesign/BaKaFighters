
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public const string MainMenuScene = "MainMenu";
    public const string FirstLevelScene = "TestLevel";
    private const int InitialLives = 3;
    private static readonly Vector2 DefaultLevelPos = new Vector2(13.46f, 0.32f);


    public SaveData[] saveSlots = new SaveData[3];

    [HideInInspector]
    public int currentSlot;
    public int playerLives = InitialLives;
    private int score = 0;
    private int highScore = 0;
    private WeaponType weaponType = WeaponType.Bullet;
    private int weaponLevel = 0;
    private float timer;
    private HashSet<string> destroyedIds = new HashSet<string>();
    private SaveData pendingLoad;
    public float StartingTimer => timer;
    public int WeaponLevel
    {
        get => weaponLevel;
        set => weaponLevel = value;
    }
    
    public WeaponType WeaponType
    {
        get => weaponType;
        set => weaponType = value;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadAllSlots();
    }


    private void Update()
    {
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.IncreaseTime();
        }
    }

    private void ResetRunState()
    {
        score = 0;
        playerLives = InitialLives;
        weaponType = WeaponType.Bullet;
        weaponLevel = 0;
        timer = 0f;
        pendingLoad = null;
        destroyedIds.Clear();
        Time.timeScale = 1f;
    }

    public void StartNewGame(int slotIndex)
    {
        ResetRunState();
        currentSlot = slotIndex;

        SaveData saveData = new SaveData
        {
            SlotIndex = slotIndex,
            LevelName = FirstLevelScene,
            Score = score,
            Timer = timer,
            PlayerLives = playerLives,
            HighScore = highScore,
            PosLevel = DefaultLevelPos,
            SaveDate = System.DateTime.Now.ToString("dd/MM/yyyy - HH:mm"),
            Weapon = weaponType,
            WeaponLevel = weaponLevel,
            IsFreshStart = true // nessun progresso: player, vita, scudo e ki partono dai valori del livello
        };
        WriteSlot(saveData);

        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();
    }

    public void AddPoint(int amount)
    {
        score += amount;

        // Se nella scena è presente un'interfaccia grafica, la aggiorniamo
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.UpdateScoreUI(score);
        }

        if (score > highScore)
        {
            highScore = score;
        }
    }

    public void TakeLife()
    {
        playerLives--;
        score = 0;
        weaponLevel = 0;
        weaponType = WeaponType.Bullet;
        timer = 0f;
        pendingLoad = null;
        destroyedIds.Clear(); // la scena si ricarica e tutti i nemici/pickup tornano
        Time.timeScale = 1f;
        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();
        if (AudioManager.instance != null)
            AudioManager.instance.RestartMusic();

        // da sistemare poi con i checkpoint, per ora resettiamo il livello
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ProcessPlayerDeath()
    {
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            playerLives = 0;
            LevelUI.Instance.ShowGameOverPanel();
        }
    }

    /// <summary>
    /// Riavvia il livello corrente, resettando il punteggio, le vite del giocatore e la musica.
    /// </summary>
    public void RestartLevel()
    {
        ResetRunState();
        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();

        if (AudioManager.instance != null)
            AudioManager.instance.RestartMusic();

        // Se c'è un LevelUI con il fade, usiamo la sua coroutine, altrimenti carichiamo direttamente
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.StartFadeOut(() =>
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            });
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    /// <summary>
    /// Ritorna al menu principale, resettando il punteggio e le vite del giocatore.
    /// </summary>
    public void ReturnMainMenu()
    {
        ResetRunState();
        if (InputManager.Instance != null)
            InputManager.Instance.EnableUI();

        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.StartFadeOut(() => {
                SceneManager.LoadScene(MainMenuScene);
            });
        }
        else
        {
            SceneManager.LoadScene(MainMenuScene);
        }
    }

    public int GetScore() => score;
    public int GetHighScore() => highScore;

    // sistemi di salvataggio e caricamento e eliminazione dei dati di salvataggio

    private string SlotPath(int slotIndex) => Application.persistentDataPath + $"/saveSlot{slotIndex}.json";
    private void WriteSlot(SaveData saveData)
    {
        saveSlots[saveData.SlotIndex] = saveData;
        File.WriteAllText(SlotPath(saveData.SlotIndex), JsonUtility.ToJson(saveData, true));
    }
    // --- SISTEMA DI SALVATAGGIO, CARICAMENTO ED ELIMINAZIONE ---
    /// <summary>
    /// Carica tutti i salvataggi dai file JSON presenti nella cartella persistente dell'applicazione.
    /// </summary>
    private void LoadAllSlots()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            string path = SlotPath(i);
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                saveSlots[i] = JsonUtility.FromJson<SaveData>(json);
            }
            else
            {
                saveSlots[i] = null;
            }
        }
    }

    // controllo se esiste almeno un salvataggio
    public bool HasAnySaveFile()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null) return true;
        }
        return false;
    }
    /// <summary>
    /// Salva il progresso del gioco in uno slot specifico.
    /// </summary>
    /// <param name="slotIndex"></param>
    public void SaveGame(int slotIndex)
    {
        // La fonte vera di vita, scudo e ki è il PlayerController: gli slider sono animati e possono essere a metà strada.
        PlayerController playerController = FindAnyObjectByType<PlayerController>();
        Vector2 playerPos = playerController != null ? (Vector2)playerController.transform.position : Vector2.zero;
        float health = playerController != null ? playerController.health : 0f;
        float shield = playerController != null ? playerController.shield : 0f;
        float ki = playerController != null ? playerController.ki : 0f;

        float currentTimer = LevelUI.Instance != null ? LevelUI.Instance.Crono : 0f;
        Vector2 levelPos = DefaultLevelPos;
        if (LevelUI.Instance != null && LevelUI.Instance.Level != null)
        {
            levelPos = LevelUI.Instance.Level.transform.position;
        }

        SaveData saveData = new SaveData
        {
            SlotIndex = slotIndex,
            LevelName = SceneManager.GetActiveScene().name,
            Score = score,
            Timer = currentTimer,
            PlayerLives = playerLives,
            PosPlayer = playerPos,
            HighScore = highScore,
            PosLevel = levelPos,
            SaveDate = System.DateTime.Now.ToString("dd/MM/yyyy - HH:mm"),
            Weapon = WeaponType,
            WeaponLevel = WeaponLevel,
            Health = health,
            Shield = shield,
            Ki = ki,
            DestroyedIds = destroyedIds != null ? new List<string>(destroyedIds) : new List<string>()
        };

        WriteSlot(saveData);

        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.ClosePausa();
        }

        Debug.Log($"Gioco salvato con successo nello Slot {slotIndex}");
    }

    public void SaveGamePausa()
    {
        SaveGame(currentSlot);
    }
    /// <summary>
    /// Carica il progresso del gioco da uno slot specifico.
    /// </summary>
    /// <param name="slotIndex"></param>
    public void LoadGame(int slotIndex)
    {
        if (saveSlots[slotIndex] == null)
        {
            Debug.LogWarning($"Impossibile caricare: lo Slot {slotIndex} è vuoto!");
            return;
        }

        currentSlot = slotIndex;
        SaveData data = saveSlots[slotIndex];

        // Ripristiniamo i valori di gioco
        score = data.Score;
        playerLives = data.PlayerLives;
        highScore = data.HighScore;
        timer = data.Timer;
        weaponType = data.Weapon;
        weaponLevel = data.WeaponLevel;
        destroyedIds = new HashSet<string>(data.DestroyedIds ?? new List<string>());
        // Gli oggetti della nuova scena (LevelUI, PlayerController) leggono questi dati da soli
        pendingLoad = data.IsFreshStart ? null : data;

        Time.timeScale = 1f;
        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();

        // I vecchi salvataggi creati dal menu hanno "MainMenu" come scena: li mandiamo al livello
        string sceneToLoad = (string.IsNullOrEmpty(data.LevelName) || data.LevelName == MainMenuScene)
            ? FirstLevelScene
            : data.LevelName;

        SceneManager.sceneLoaded += OnSceneLoadedRestoreWeapon;
        SceneManager.LoadScene(sceneToLoad);
    }
    /// <summary>
    /// Metodo chiamato quando una scena viene caricata, per ripristinare la posizione del giocatore e altri dati salvati.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    /// 
    public bool TryGetLoadedState(out SaveData data)
    {
        data = pendingLoad;
        return data != null;
    }

    private void OnSceneLoadedRestoreWeapon(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedRestoreWeapon;
        if (pendingLoad != null)
            StartCoroutine(RestoreWeaponAfterFrame(pendingLoad));
    }

    public void LoadGamePausa()
    {
        LoadGame(currentSlot);
    }


    /// <summary>
    /// Elimina il salvataggio in uno slot specifico, sia dalla memoria che dal file JSON.
    /// </summary>
    /// <param name="slotIndex"></param>
    public void DeleteGame(int slotIndex)
    {
        saveSlots[slotIndex] = null;
        string path = SlotPath(slotIndex);

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Salvataggio nello Slot {slotIndex} eliminato.");
        }
    }


    public void RegisterDestroyed(string id)
    {
        if (destroyedIds != null)
        {
            destroyedIds.Add(id);
        }
    }

    public bool IsDestroyed(string id)
    {
        return destroyedIds != null && destroyedIds.Contains(id);
    }


    private IEnumerator RestoreWeaponAfterFrame(SaveData data)
    {
        yield return null; // aspetta un frame: gli Start sono già stati eseguiti

        WeaponController weaponController = FindAnyObjectByType<WeaponController>();
        if (weaponController != null)
        {
            weaponController.SetWeapon(data.Weapon, data.WeaponLevel);
            WeaponUI weaponUI = FindAnyObjectByType<WeaponUI>();

            if (weaponUI != null)
            {
                weaponUI.RestoreWeaponUI(data.Weapon, data.WeaponLevel, weaponController.weaponsData);
            }
            else
            {
                Debug.Log("WeaponUI non trovato in scena.");
            }
        }
        else
        {
            Debug.LogWarning("WeaponController non trovato in scena!");
        }

        pendingLoad = null; // il caricamento è completato
    }

}

[System.Serializable]
public class SaveData
{
    public int SlotIndex;
    public string SaveDate;
    public string LevelName;
    public int Score;
    public float Timer;
    public int PlayerLives;
    public int HighScore;
    // true per uno slot appena creato da "Nuova partita": non ha ancora un progresso da ripristinare
    public bool IsFreshStart;
    //Da valutare se usare un componente esterno come il checkpoint per questi dati, per ora li salviamo direttamente nel salvataggio
    public Vector2 PosPlayer = new Vector2(0, 0);
    public Vector2 PosLevel = new Vector2(13.46f, 0.32f);
    public WeaponType Weapon = WeaponType.Bullet;
    public int WeaponLevel = 0;
    public float Health;
    public float Shield;
    public float Ki;

    public List<string> DestroyedIds = new List<string>();
}