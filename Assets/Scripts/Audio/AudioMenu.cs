using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioMenu : MonoBehaviour, ISelectHandler, ISubmitHandler, IPointerClickHandler, IPointerEnterHandler
{
    public void OnSelect(BaseEventData eventData)
    {
        if (AudioManager.instance.SFXSource != null && AudioManager.instance.select != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.select);
        }
    }

    public void OnSubmit(BaseEventData eventData)
    {
        PlaySubmitSound();
    }

    public void OnCancel(BaseEventData eventData)
    {
        PlayCancelSound();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySubmitSound();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(gameObject);
        }
    }


    private void PlaySubmitSound()
    {
        if (AudioManager.instance.SFXSource != null && AudioManager.instance.confirm != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.confirm);
        }
    }

    private void PlayCancelSound()
    {
        if (AudioManager.instance.SFXSource != null && AudioManager.instance.cancel != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.cancel);
        }
    }


}
