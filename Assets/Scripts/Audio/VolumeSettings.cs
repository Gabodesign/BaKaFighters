using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSettings : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider SFXSlider;
    [SerializeField] private Toggle mute;

    private void Start()
    {
        if (PlayerPrefs.HasKey("isMuted"))
        {
            mute.isOn = PlayerPrefs.GetInt("isMuted") == 1;
        }

        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
            SetSFXVolume();
        }
    }

    public void SetMusicVolume()
    {
        float volume = musicSlider.value;

        // Salviamo nelle PlayerPrefs il volume SOLO se non è zero (così non sovrascriviamo il volume reale quando mutiamo)
        if (volume > 0.0001f)
        {
            PlayerPrefs.SetFloat("musicVolume", volume);
        }

        // Se il toggle Mute è attivo, forziamo il mixer a -80f indipendentemente dallo slider
        if (mute.isOn)
        {
            audioMixer.SetFloat("Music", -80f);
            return;
        }

        audioMixer.SetFloat("Music", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
    }

    public void SetSFXVolume()
    {
        float volume = SFXSlider.value;

        // Salviamo nelle PlayerPrefs il volume SOLO se non è zero
        if (volume > 0.0001f)
        {
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        // Se il toggle Mute è attivo, forziamo il mixer a -80f indipendentemente dello slider
        if (mute.isOn)
        {
            audioMixer.SetFloat("SFX", -80f);
            return;
        }

        audioMixer.SetFloat("SFX", Mathf.Log10(Mathf.Max(0.0001f, volume)) * 20);
    }

    public void SetMute(bool isMuted)
    {
        PlayerPrefs.SetInt("isMuted", isMuted ? 1 : 0);

        if (isMuted)
        {
            // Spegniamo l'audio nel Mixer
            audioMixer.SetFloat("Music", -80f);
            audioMixer.SetFloat("SFX", -80f);

            // AGGIUNTA: Portiamo gli slider visivi a 0 per dare il feedback grafico
            musicSlider.value = 0f;
            SFXSlider.value = 0f;

            Debug.Log("Audio Muted & Sliders Zeroed");
        }
        else
        {
            // Se togliamo il muto, andiamo a riprendere i valori reali salvati prima del muto
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume", 0.75f);
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 0.75f);

            // Riapplichiamo i volumi corretti al mixer
            SetMusicVolume();
            SetSFXVolume();
            Debug.Log("Audio Unmuted & Sliders Restored");
        }
    }

    private void LoadVolume()
    {
        if (mute.isOn)
        {
            // Se all'avvio era mutato, gli slider visivi vanno a 0 e il mixer a -80f
            musicSlider.value = 0f;
            SFXSlider.value = 0f;
            audioMixer.SetFloat("Music", -80f);
            audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            // Altrimenti carichiamo i valori normali
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume");
            SetMusicVolume();
            SetSFXVolume();
        }
    }
}