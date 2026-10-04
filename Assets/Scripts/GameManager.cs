
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public SaveData[] saveSlots = new SaveData[3];

    [HideInInspector]
    public int currentSlot;
    public int playerLives = 3;
    private int score = 0;
    private int highScore = 0;
    private WeaponType weaponType = WeaponType.Bullet;
    private int weaponLevel = 0;
    private float timer;
    private float health;
    private float shield;
    private float ki;

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
        // Se nella scena è presente un'interfaccia grafica, la aggiorniamo
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
        Time.timeScale = 1f;
        if (InputManager.Instance != null)
            InputManager.Instance.EnableGameplay();
        score = 0; // Resettiamo il punteggio per la nuova partita
        playerLives = 3;
        weaponLevel = 0;
        weaponType = WeaponType.Bullet;

        if (AudioManager.instance != null)
            AudioManager.instance.RestartMusic();

        // Se c'è un LevelUI con il fade, usiamo la sua coroutine, altrimenti carichiamo direttamente
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.ResetTime();
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
        Time.timeScale = 1f;
        if (InputManager.Instance != null)
            InputManager.Instance.EnableUI();
        score = 0;

        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.StartFadeOut(() => {
                SceneManager.LoadScene("MainMenu");
            });
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public int GetScore() => score;
    public int GetHighScore() => highScore;

    // sistemi di salvataggio e caricamento e eliminazione dei dati di salvataggio

    // --- SISTEMA DI SALVATAGGIO, CARICAMENTO ED ELIMINAZIONE ---
    /// <summary>
    /// Carica tutti i salvataggi dai file JSON presenti nella cartella persistente dell'applicazione.
    /// </summary>
    private void LoadAllSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            string path = Application.persistentDataPath + $"/saveSlot{i}.json";
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
        //recuperiamo la posizione del player
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 playerPos = player != null ? player.transform.position : Vector2.zero; // settiamo a 0,0 la posizione se non troviamo l'oggetto con il tag Player
        //recuperiamo la posizione del livello
        Vector2 levelPos = new Vector2(13.46f, 0.32f); // settiamo di default la posizione del livello se è null

        health = UIController.Instance != null ? UIController.Instance.HealthValue: 0f;
        shield = UIController.Instance != null ? UIController.Instance.ShieldValue : 0f;
        ki = UIController.Instance != null ? UIController.Instance.KiValue : 0f;

        float currentTimer = LevelUI.Instance != null ? LevelUI.Instance.Crono : 0f;

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
            Ki = ki
        };

        saveSlots[slotIndex] = saveData;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + $"/saveSlot{slotIndex}.json", json);
        Debug.Log($"{saveData.SlotIndex} {saveData.LevelName} {saveData.Score} {saveData.PlayerLives} {saveData.PosPlayer} {saveData.HighScore} {saveData.PosLevel} {saveData.SaveDate} ");

        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.panelPausa.SetActive(false);
            Time.timeScale = 1f;
            if (InputManager.Instance != null)
                InputManager.Instance.EnableGameplay();
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
        if (saveSlots[slotIndex] != null)
        {
            currentSlot = slotIndex;
            SaveData data = saveSlots[slotIndex];

            // Ripristiniamo i valori di gioco
            score = data.Score;
            playerLives = data.PlayerLives;
            highScore = data.HighScore;
            timer = data.Timer;
            weaponType = data.Weapon;
            weaponLevel = data.WeaponLevel;

            Debug.Log($"{data.SlotIndex} {data.LevelName} {data.Score} {data.PlayerLives} {data.PosPlayer} {data.HighScore} {data.PosLevel} {data.SaveDate} ");

            Time.timeScale = 1f;
            if(InputManager.Instance != null)
                InputManager.Instance.EnableGameplay();

            // Carichiamo la scena memorizzata nel salvataggio
            SceneManager.sceneLoaded += OnSceneLoadedRestorePlayer;
            SceneManager.LoadScene(data.LevelName);
        }
        else
        {
            Debug.LogWarning($"Impossibile caricare: lo Slot {slotIndex} è vuoto!");
        }
    }
    /// <summary>
    /// Metodo chiamato quando una scena viene caricata, per ripristinare la posizione del giocatore e altri dati salvati.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    private void OnSceneLoadedRestorePlayer(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedRestorePlayer;

        SaveData data = saveSlots[currentSlot];
        if (data == null)
        {
            Debug.LogWarning("OnSceneLoaded: SaveData è null!");
            return;
        }

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.PosPlayer;
        }
        else
        {
            Debug.LogWarning("Player non trovato nella scena!");
        }

        if (LevelUI.Instance != null && LevelUI.Instance.Level != null)
        {
            LevelUI.Instance.Level.transform.position = data.PosLevel;
        }

        // --- Ripristino timer, spostato qui invece che in RestoreAfterFrame ---
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.RestoreTime(data.Timer); // nota: data.Timer, non il campo timer
        }

        weaponType = data.Weapon;
        weaponLevel = data.WeaponLevel;

        StartCoroutine(RestoreAfterFrame());
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
        string path = Application.persistentDataPath + $"/saveSlot{slotIndex}.json";

        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"Salvataggio nello Slot {slotIndex} eliminato.");
        }
    }


    private IEnumerator RestoreAfterFrame()
    {
        yield return null; // aspetta un frame

        SaveData data = saveSlots[currentSlot];
        if (data == null) yield break;

        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.transform.position = data.PosPlayer;
            // --- Ripristino vita / scudo / ki ---
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.health = data.Health;
                playerController.shield = data.Shield;
                playerController.ki = data.Ki;

                if (UIController.Instance != null)
                {
                    UIController.Instance.UpdateHealthSlider(data.Health, playerController.maxHealth);
                    UIController.Instance.UpdateShieldSlider(data.Shield, playerController.maxShield);
                    UIController.Instance.UpdateKiSlider(data.Ki, playerController.maxKi);
                }
            }

            WeaponController weaponController = player.GetComponentInChildren<WeaponController>();
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
                Debug.LogWarning("WeaponController non trovato sul player!");
            }
        }
        else
        {
            Debug.LogWarning("Player non trovato in scena!");
        }

        if (LevelUI.Instance != null && LevelUI.Instance.Level != null)
            LevelUI.Instance.Level.transform.position = data.PosLevel;
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
    //Da valutare se usare un componente esterno come il checkpoint per questi dati, per ora li salviamo direttamente nel salvataggio
    public Vector2 PosPlayer = new Vector2(0, 0);
    public Vector2 PosLevel = new Vector2(13.46f, 0.32f);
    public WeaponType Weapon = WeaponType.Bullet;
    public int WeaponLevel = 0;
    public float Health;
    public float Shield;
    public float Ki;
    public List<Enemy> Enemies; // da completare, per ora non lo usiamo, ma in futuro potremmo salvare anche i nemici che ho già eliminato dal livello.
}