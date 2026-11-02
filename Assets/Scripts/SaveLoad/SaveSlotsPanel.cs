using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SaveSlotsPanel : MonoBehaviour
{
    [SerializeField] public GameObject filledSlotPrefab;
    [SerializeField] public GameObject emptySlotPrefab;
    [SerializeField] public Transform container;
    [SerializeField] public MenuManager menuManager;

    public void Populate(MenuManager.MainMenuSlotMode mode, System.Action<int> onSlotSelected)
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
            slotUI.Setup(i, data, mode, onSlotSelected);
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
                if (menuManager != null) menuManager.SelectFirstButton(slotUI.MainButton.gameObject);
                else StartCoroutine(SelectNextFrame(slotUI.MainButton.gameObject));
                return;
            }
        }
    }

    // Il pannello usato in pausa non ha un MenuManager: seleziona il pulsante da solo
    private IEnumerator SelectNextFrame(GameObject target)
    {
        yield return null;
        if (target == null || EventSystem.current == null) yield break;
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(target);
    }

    public Button GetSlotButton(int index)
    {
        if (index < 0 || index >= container.childCount) return null;

        var slotUI = container.GetChild(index).GetComponent<SaveSlotUI>();
        return slotUI != null ? slotUI.MainButton : null;
    }
}