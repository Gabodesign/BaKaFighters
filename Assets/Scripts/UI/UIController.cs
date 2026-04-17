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

    public void UpdateHealthSlider(float current, float max) 
    {
        health.maxValue = max;                      
        health.value =current;    
    }

    public void UpdateShieldSlider(float current, float max) 
    {
        shield.maxValue = max;                       
        shield.value = current;    
    }

    public void UpdateKiSlider(float current, float max) 
    {
        ki.maxValue = max;                           
        ki.value = current;        
    }
}
