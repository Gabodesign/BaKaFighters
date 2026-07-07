using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AudioMenu : MonoBehaviour,
    ISelectHandler, IDeselectHandler,
    ISubmitHandler, IPointerClickHandler,
    IPointerEnterHandler
{
    [Header("Audio")]
    public AudioSource audioSource;              
    public AudioClip navigateClip;               
    public AudioClip submitClip;

    public void OnSelect(BaseEventData eventData)
    {
        if (audioSource != null && navigateClip != null)
        {
            audioSource.PlayOneShot(navigateClip);
        }
    }
    public void OnDeselect(BaseEventData eventData)
    {
        throw new System.NotImplementedException();
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
        if (audioSource != null && submitClip != null)
        {
            audioSource.PlayOneShot(submitClip);
        }
    }

    private void Awake()
    {
        if (audioSource == null)
        {
            // Prova a prenderlo sullo stesso GameObject.
            audioSource = GetComponent<AudioSource>();

            // Se non c'è, prova a cercarlo in un parent (es. Canvas).
            if (audioSource == null)
                audioSource = GetComponentInParent<AudioSource>();
        }
    }


}
