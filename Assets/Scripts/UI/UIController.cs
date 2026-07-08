using DG.Tweening;
using TMPro;                                         
using UnityEngine;
using UnityEngine.EventSystems;  
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance;
    [Header("UI References")]
    [SerializeField] private Slider health;        
    [SerializeField] private Slider shield;          
    [SerializeField] private Slider ki;
    [SerializeField] private TextMeshProUGUI scoreText;



    private void Awake()                             
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        
    }

    public void UpdateHealthSlider(float current, float max)
    {
        if (health == null) return;

        // Questo ci dirà nella console se la funzione viene chiamata troppe volte o con valori errati
        Debug.Log($"UI: Cambio vita richiesto. Valore Corrente Slider: {health.value} -> Obiettivo: {current}");

        health.maxValue = max;
        health.DOKill();
        health.DOValue(current, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);
    }

    public void UpdateShieldSlider(float current, float max)
    {
        if (shield == null) return;

        shield.maxValue = max;

        shield.DOKill();
        shield.DOValue(current, 0.2f).SetEase(Ease.OutQuad);
    }

    public void UpdateKiSlider(float current, float max)
    {
        if (ki == null) return;

        ki.maxValue = max;

        ki.DOKill();
        ki.DOValue(current, 0.2f).SetEase(Ease.OutQuad);
    }
}
