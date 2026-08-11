using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveSlotsPanel : MonoBehaviour
{
    [SerializeField] public GameObject filledSlotPrefab;
    [SerializeField] public GameObject emptySlotPrefab;
    [SerializeField] public Transform container;
    [SerializeField] public MenuManager menuManager;

    public void Populate(MenuManager.MainMenuSlotMode mode)
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        SaveData[] saveSlots = GameManager.Instance.saveSlots;
        List<SaveSlotUI> newSlots = new List<SaveSlotUI>();

        for (int i = 0; i < saveSlots.Length; i++)
        {
            SaveData data = saveSlots[i];
            bool hasData = data != null;

            GameObject prefabToUse = hasData ? filledSlotPrefab : emptySlotPrefab;
            GameObject slotGO = Instantiate(prefabToUse, container);

            var slotUI = slotGO.GetComponent<SaveSlotUI>();
            slotUI.Setup(i, data, mode, menuManager);
            newSlots.Add(slotUI);
        }

        SelectFirstInteractableSlot(newSlots);
    }

    private void SelectFirstInteractableSlot(List<SaveSlotUI> slots)
    {
        foreach (var slotUI in slots)
        {
            if (slotUI != null && slotUI.MainButton != null && slotUI.MainButton.interactable)
            {
                menuManager.SelectFirstButton(slotUI.MainButton.gameObject);
                return;
            }
        }
    }

    public Button GetSlotButton(int index)
    {
        if (index < 0 || index >= container.childCount) return null;

        var slotUI = container.GetChild(index).GetComponent<SaveSlotUI>();
        return slotUI != null ? slotUI.MainButton : null;
    }
}