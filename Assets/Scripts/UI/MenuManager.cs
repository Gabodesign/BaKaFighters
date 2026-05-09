using UnityEngine;
using UnityEngine.UI;                  
using UnityEngine.SceneManagement;
using System.Collections;
public class MenuManager : MonoBehaviour
{

    [Header("Fade")]
    public Image fadeImage;                        
    public float fadeDuration = 0.3f;
    private bool isTransitioning;

    [Header("Scena di gioco")]
    public string loadingSceneName = "Level";

    private void Start()
    {
        
        if (fadeImage != null)
        {
            Color colore = fadeImage.color;  
            colore.a = 0f;                   
            fadeImage.color = colore;        

            fadeImage.raycastTarget = false;  
            fadeImage.gameObject.SetActive(true); 
        }
    }

    public void OnLevelScene()
    {
        StartCoroutine(LoadSceneWithFade(loadingSceneName));
    }

    
    private IEnumerator LoadSceneWithFade(string sceneName)
    {
        isTransitioning = true;                 // Segniamo che è in corso una transizione.

        if (fadeImage == null)
        {
            // Se per qualche motivo non abbiamo un fadeImage, carichiamo direttamente.
            SceneManager.LoadScene(sceneName);
            yield break;
        }

        fadeImage.raycastTarget = true;         // Blocchiamo input durante il fade.

        Color c = fadeImage.color;
        for (float t = 0f; t < fadeDuration; t += Time.unscaledDeltaTime)
        {
            float alpha = t / fadeDuration;     // 0 -> 1
            c.a = alpha;
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;           // Assicuriamo che sia completamente nero.
        fadeImage.color = c;

        // Carichiamo la scena (es. "Loading").
        SceneManager.LoadScene(sceneName);
    }
}
