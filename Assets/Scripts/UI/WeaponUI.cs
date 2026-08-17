using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image iconWeapon;
    [SerializeField] private Image[] levelPips;
    private bool isWeaponUIActive = false;

    public void UpdateWeaponUI(int levelWeapon, WeaponData[] weaponData)
    {
        if (iconWeapon == null || isWeaponUIActive) return;

        foreach (var weapon in weaponData)
        {
            iconWeapon.sprite = weapon.icon;
            Debug.Log("Aggiornamento UI Arma " + levelWeapon + "icona dell'arma: " + iconWeapon.name);
            isWeaponUIActive = true;
        }

    }
}
