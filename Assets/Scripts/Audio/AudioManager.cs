using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] public AudioSource musicSource;
    [SerializeField] public AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip musicMainMenu;
    public AudioClip musicLevel;
    public AudioClip select;
    public AudioClip confirm;
    public AudioClip cancel;
    public AudioClip pause;
    public AudioClip unpause;
    public AudioClip bullet;
    public AudioClip hit;

    private AudioClip music;

    private void Awake()
    {
        if (instance == null) 
        {
            instance = this;    
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AudioClip musicDaRiprodurre = null;

        if (scene.name == "MainMenu")
        {
            musicDaRiprodurre = musicMainMenu;
        }
        else if (scene.name == "TestLevel")
        {
            musicDaRiprodurre = musicLevel;
        }

        if (musicDaRiprodurre != null && musicSource.clip != musicDaRiprodurre)
        {
            musicSource.Stop();
            musicSource.clip = musicDaRiprodurre;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }
}
