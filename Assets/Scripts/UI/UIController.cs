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

    public float HealthValue => health.value;
    public float ShieldValue => shield.value;
    public float KiValue => ki.value;

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

    public void UpdateHealthSlider(float current, float max, bool instant = false)
    {
        SetSlider(health, current, max, instant);
    }

    public void UpdateShieldSlider(float current, float max, bool instant = false)
    {
        SetSlider(shield, current, max, instant);
    }

    public void UpdateKiSlider(float current, float max, bool instant = false)
    {
        SetSlider(ki, current, max, instant);
    }

    private void SetSlider(Slider slider, float current, float max, bool instant)
    {
        if (slider == null) return;

        slider.DOKill();
        slider.maxValue = max;

        if (instant)
        {
            slider.value = current;
            return;
        }
        slider.DOValue(current, 0.2f).SetEase(Ease.OutQuad).SetUpdate(true);
    }
}
