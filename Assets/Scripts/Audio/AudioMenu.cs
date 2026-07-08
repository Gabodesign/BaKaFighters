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

    private void Awake()
    {
        if (AudioManager.instance.SFXSource == null)
        {
            Debug.LogError("Manca associazione del AudioSource");
        }
    }

}
