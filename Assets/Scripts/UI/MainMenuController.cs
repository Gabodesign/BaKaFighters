using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class MainMenuController : MonoBehaviour
{
    public List<Button> mainMenuButtons;
    public List<Toggle> optionsButtons;
    public List<Button> newGamePopupButtons;
    public List<Button> loadGamePopupButtons;
    public List<Button> deleteGamePopupButtons;
    public List<Button> exitGamePopupButtons;

    public bool isLoad;
    public EventSystem eventSystem;



    IEnumerator SelectFirstButton()
    {
        yield return null;
        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }

        eventSystem.SetSelectedGameObject(null);

        eventSystem.SetSelectedGameObject(mainMenuButtons[0].gameObject);

        SetCursorLocked(true);
    }

    private void SetCursorLocked(bool locked)
    {
        if (locked)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
