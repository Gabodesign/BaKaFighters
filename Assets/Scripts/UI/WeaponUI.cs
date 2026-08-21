using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WeaponUI : MonoBehaviour
{
    [SerializeField] private Image iconWeapon;
    [SerializeField] private Image[] levelPips; // 3 pip, uno per livello
    [SerializeField] private float fadeDuration = 0.3f;

    private WeaponType currentWeaponType;
    private bool initialized = false;

    public void UpdateWeaponUI(WeaponType weaponType, int levelWeapon, WeaponData[] weaponData)
    {
        if (iconWeapon == null) return;

        // Cambio arma: aggiorna icona e resetta i pip
        if (!initialized || currentWeaponType != weaponType)
        {
            currentWeaponType = weaponType;
            iconWeapon.sprite = weaponData[(int)weaponType].icon;
            ResetPips();
            initialized = true;
        }

        // pip[0] è già acceso di default (livello base), niente fade
        // Solo dal livello 1 in su facciamo il fade-in
        if (levelWeapon >= 1 && levelWeapon < levelPips.Length)
        {
            StartCoroutine(FadeInPip(levelPips[levelWeapon]));
        }
    }

    private void ResetPips()
    {
        for (int i = 0; i < levelPips.Length; i++)
        {
            SetAlpha(levelPips[i], i == 0 ? 1f : 0f); // pip[0] acceso subito, gli altri spenti
        }
    }

    private IEnumerator FadeInPip(Image pip)
    {
        float elapsed = 0f;
        float startAlpha = pip.color.a;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(pip, Mathf.Lerp(startAlpha, 1f, elapsed / fadeDuration));
            yield return null;
        }

        SetAlpha(pip, 1f);
    }

    private void SetAlpha(Image image, float alpha)
    {
        Color c = image.color;
        c.a = alpha;
        image.color = c;
    }
}