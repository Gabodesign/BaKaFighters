using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }
    [SerializeField] private TMP_Text scoreText;
    private int score = 0;
    private int highScore = 0;

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
    }

    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    void UpdateUI()
    {
        if (scoreText != null) scoreText.text = score.ToString("N0");
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
