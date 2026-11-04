using UnityEngine;

public static class Bootstrapper 
{
    private static readonly string[] managerPrefabs = { "GameManager", "InputManager", "AudioManager" };

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Initialize()
    {
        foreach (string prefabName in managerPrefabs)
        {
            GameObject prefab = Resources.Load<GameObject>(prefabName);
            if (prefab != null)
            {
                GameObject instance = Object.Instantiate(prefab);
                instance.name = prefabName;
            }
            else
            {
                Debug.LogError($"{prefabName} prefab not found in Resources folder.");
            }
        }
    }
}
