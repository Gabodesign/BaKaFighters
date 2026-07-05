using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Cinemachine.DocumentationSortingAttribute;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int score = 0;
    private int highScore = 0;

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


    public void RestartLevel()
    {
        Time.timeScale = 1f;
        score = 0; // Resettiamo il punteggio per la nuova partita

        // Se c'è un LevelUI con il fade, usiamo la sua coroutine, altrimenti carichiamo direttamente
        if (LevelUI.Instance != null)
        {
            LevelUI.Instance.StartFadeOut(() => {
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