using System;
using UnityEngine;

public class WaveEnemy : MonoBehaviour
{
    public event Action<WaveEnemy> Destroyed;

    private void OnDestroy()
    {
        // Durante lo scaricamento della scena (morte, menu) l'ondata non va più aggiornata
        if (!gameObject.scene.isLoaded) return;
        Destroyed?.Invoke(this);
    }
}
