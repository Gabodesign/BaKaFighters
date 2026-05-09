using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitEffect : MonoBehaviour
{
    [Header("Default (se non passati a runtime)")]
    public Color defaultColor = new Color(1f, 1f, 1f, 0.5f); 
    public float defaultDuration = 0.08f;                    

    private List<SpriteRenderer> sprites = new List<SpriteRenderer>(); 
    private List<Color> spriteOriginals = new List<Color>();           

    private SkeletonRenderer spineRenderer; 
    private Color spineOriginal = Color.white; 

    private Coroutine running;

    void Awake()
    {
        
        var found = GetComponentsInChildren<SpriteRenderer>(true); 
        sprites.AddRange(found);                                   
        for (int i = 0; i < sprites.Count; i++)                    
            spriteOriginals.Add(sprites[i].color);                 


        spineRenderer = GetComponentInChildren<SkeletonRenderer>(true); 
        if (spineRenderer != null && spineRenderer.Skeleton != null)   
        {
            spineOriginal = spineRenderer.Skeleton.GetColor();         
        }
    }


    public void FlashOnce(Color? color = null, float? duration = null)
    {
        
        Color c = color ?? defaultColor;      
        float d = duration ?? defaultDuration;

        if (running != null) StopCoroutine(running);

        running = StartCoroutine(DoFlash(c, d));
    }


    IEnumerator DoFlash(Color flashColor, float duration)
    {
       
        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] != null)           
                sprites[i].color = flashColor; 
        }

        if (spineRenderer != null && spineRenderer.Skeleton != null)
        {
            spineRenderer.Skeleton.SetColor(flashColor); 
            spineRenderer.LateUpdate();                 
        }


        yield return new WaitForSeconds(duration);

        for (int i = 0; i < sprites.Count; i++)
        {
            if (sprites[i] != null)                     
                sprites[i].color = spriteOriginals[i];  
        }


        if (spineRenderer != null && spineRenderer.Skeleton != null)
        {
            spineRenderer.Skeleton.SetColor(spineOriginal); 
            spineRenderer.LateUpdate();                     
        }

        running = null;
    }
}
