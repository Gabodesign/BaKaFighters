using UnityEngine;

public static class PersistenManager
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    static void Initialize()
    {
        GameObject persistentManager = Resources.Load<GameObject>("GameManager");
        if (persistentManager != null)
        {
            GameObject instance = Object.Instantiate(persistentManager);
            instance.name = "GameManager";
            Object.DontDestroyOnLoad(instance);
        }
        else
        {
            Debug.LogError("GameManager prefab not found in Resources folder.");
        }
    }
}
