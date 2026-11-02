using UnityEngine;

public class SaveableObject : MonoBehaviour
{
    [SerializeField] private string id;
    public string Id => id;

    private void Awake()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsDestroyed(id))
        {
            Destroy(gameObject);
        }
    }

    /// <summary>Da chiamare quando il nemico muore o il pickup viene raccolto (non quando esce dallo schermo).</summary>
    public void MarkDestroyed()
    {
        if (GameManager.Instance != null && !string.IsNullOrEmpty(id))
        {
            GameManager.Instance.RegisterDestroyed(id);
        }
    }

#if UNITY_EDITOR
    // Si genera da solo quando aggiungi il componente, e si rigenera se un oggetto copiato ha l'id di un altro
    private void OnValidate()
    {
        // Sul prefab (asset) non vogliamo un id: ce l'ha ogni istanza in scena
        if (!gameObject.scene.IsValid() || Application.isPlaying) return;

        bool duplicated = false;
        if (!string.IsNullOrEmpty(id))
        {
            foreach (SaveableObject other in FindObjectsByType<SaveableObject>(FindObjectsSortMode.None))
            {
                if (other != this && other.id == id) { duplicated = true; break; }
            }
        }

        if (string.IsNullOrEmpty(id) || duplicated)
        {
            GenerateId();
        }
    }

    [ContextMenu("Rigenera ID")]
    private void GenerateId()
    {
        id = System.Guid.NewGuid().ToString();
        UnityEditor.EditorUtility.SetDirty(this);
    }
#endif
}
