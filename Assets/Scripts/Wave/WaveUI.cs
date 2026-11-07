using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WaveUI : MonoBehaviour
{
    [Header("Scritte")]
    [SerializeField] private GameObject warningObject;
    [SerializeField] private GameObject waveCompleteObject;
    [SerializeField] private GameObject continueObject; // scritta Continue + freccia
    [SerializeField] private float flashInterval = 0.25f;

    [Header("Contatori")]
    [SerializeField] private TMP_Text waveCounterText;
    [SerializeField] private TMP_Text enemyCounterText;

    private Coroutine warningRoutine;

    private void Awake()
    {
        SetActive(warningObject, false);
        SetActive(waveCompleteObject, false);
        SetActive(continueObject, false);
    }
    public void StartWarning()
    {
        StopWarning();
        warningRoutine = StartCoroutine(FlashStopped(warningObject));
    }

    public void StopWarning()
    {
        if (warningRoutine != null)
        {
            StopCoroutine(warningRoutine);
            warningRoutine = null;
        }
        SetActive(warningObject, false);
    }

    private IEnumerator FlashStopped(GameObject target)
    {
        bool visible = true;
        while (true)
        {
            SetActive(target, visible);
            visible = !visible;
            yield return new WaitForSeconds(flashInterval);
        }
    }

    public IEnumerator ShowWaveComplete(float duration)
    {
        yield return Flash(waveCompleteObject, duration);
    }

    // Continue resta acceso fisso per qualche secondo
    public IEnumerator ShowContinue(float duration)
    {
        SetActive(continueObject, true);
        yield return new WaitForSeconds(duration);
        SetActive(continueObject, false);
    }



    public void ShowWaves() => SetActive(waveCounterText != null ? waveCounterText.gameObject : null, true);
    public void HideWaves() => SetActive(waveCounterText != null ? waveCounterText.gameObject : null, false);

    public void ShowEnemies() => SetActive(enemyCounterText != null ? enemyCounterText.gameObject : null, true);
    public void HideEnemies() => SetActive(enemyCounterText != null ? enemyCounterText.gameObject : null, false);
    public void SetWaves(int current, int total)
    {
        if (waveCounterText != null)
            waveCounterText.text = $"WAVE {current}/{total}";
    }

    public void SetEnemies(int killed, int total)
    {
        if (enemyCounterText != null)
            enemyCounterText.text = $"ENEMIES {killed}/{total}";
    }

    private IEnumerator Flash(GameObject target, float duration)
    {
        float time = 0f;
        bool visible = true;
        while (time < duration)
        {
            SetActive(target, visible);
            visible = !visible;
            yield return new WaitForSeconds(flashInterval);
            time += flashInterval;
        }
        SetActive(target, false);
    }
    private static void SetActive(GameObject go, bool value)
    {
        if (go != null) go.SetActive(value);
    }
}
