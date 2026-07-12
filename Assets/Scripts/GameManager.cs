using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
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

    
}