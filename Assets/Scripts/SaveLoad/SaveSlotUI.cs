using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotUI : MonoBehaviour
{
    [Header("Riferimenti UI (solo nel prefab 'filled')")]
    [SerializeField] private TextMeshProUGUI levelNameText;
    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI dateText;

    [SerializeField] private Button button;

    public void Setup(int index, SaveData data, MenuManager.MainMenuSlotMode mode, MenuManager menuManager)
    {
        bool hasData = data != null;

        if (hasData)
        {
            if (levelNameText != null) levelNameText.text = data.levelName;
            if (livesText != null) livesText.text = $"Lives: {data.playerLives}";
            if (scoreText != null) scoreText.text = $"Score: {data.score}";
            if (dateText != null) dateText.text = data.saveDate;
        }

        if (mode == MenuManager.MainMenuSlotMode.NewGame)
        {
            button.interactable = true; // sia vuoti che pieni: gestisce l'overwrite il popup
        }
        else // LoadGame
        {
            button.interactable = hasData; // solo slot pieni cliccabili
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => menuManager.OnSelectSlot(index));
    }
}
