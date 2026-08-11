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
    private bool gameover;

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
            gameover = true;
            LevelUI.Instance.ShowGameOverPanel();
        }
    }


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        score = 0; // Resettiamo il punteggio per la nuova partita
        playerLives = 3;
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

    public void ReturnMainMenu()
    {
        Time.timeScale = 1f;
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

    public bool HasAnySaveFile()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (saveSlots[i] != null) return true;
        }
        return false;
    }

    public void SaveGame(int slotIndex)
    {
        SaveData saveData = new SaveData
        {
            slotIndex = slotIndex,
            levelName = SceneManager.GetActiveScene().name,
            score = score,
            playerLives = playerLives,
            highScore = highScore,
            saveDate = System.DateTime.Now.ToString("dd/MM/yyyy - HH:mm")
        };

        saveSlots[slotIndex] = saveData;

        string json = JsonUtility.ToJson(saveData, true);
        File.WriteAllText(Application.persistentDataPath + $"/saveSlot{slotIndex}.json", json);

        Debug.Log($"Gioco salvato con successo nello Slot {slotIndex}");
    }

    public void LoadGame(int slotIndex)
    {
        if (saveSlots[slotIndex] != null)
        {
            SaveData data = saveSlots[slotIndex];

            // Ripristiniamo i valori di gioco
            score = data.score;
            playerLives = data.playerLives;
            highScore = data.highScore;

            Time.timeScale = 1f;

            // Carichiamo la scena memorizzata nel salvataggio
            SceneManager.LoadScene(data.levelName);
        }
        else
        {
            Debug.LogWarning($"Impossibile caricare: lo Slot {slotIndex} è vuoto!");
        }
    }

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

}

[System.Serializable]
public class SaveData
{
    public int slotIndex;
    public string saveDate;
    public string levelName;
    public int score;
    public int playerLives;
    public int highScore;
    //Da valutare se usare un componente esterno come il checkpoint per questi dati, per ora li salviamo direttamente nel salvataggio
    public Vector2 posPlayer = new Vector2(0, 0);
    public WeaponType weapon = WeaponType.Bullet;
    public int weaponLevel = 0;
    public float healt;
    public float shield;
    public float ki;
}