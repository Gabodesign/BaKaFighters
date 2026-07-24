using UnityEngine;

public class SaveSlotsPanel : MonoBehaviour
{
    [SerializeField] private GameObject filledSlotPrefab;
    [SerializeField] private GameObject emptySlotPrefab;
    [SerializeField] private Transform container;
    [SerializeField] private MenuManager menuManager;

    public void Populate(MenuManager.MainMenuSlotMode mode)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        SaveData[] saveSlots = GameManager.Instance.saveSlots;

        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveData data = saveSlots[i];
            bool hasData = data != null;

            GameObject prefabToUse = hasData ? filledSlotPrefab : emptySlotPrefab;
            GameObject slotGO = Instantiate(prefabToUse, container);

            slotGO.GetComponent<SaveSlotUI>().Setup(i, data, mode, menuManager);
        }
    }
}