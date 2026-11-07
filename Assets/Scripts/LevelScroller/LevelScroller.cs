using UnityEngine;
public class LevelScroller : MonoBehaviour
{
    public static LevelScroller Instance { get; private set; }

    [SerializeField] private float speed = 1f;
    [SerializeField] private bool scrolling = true;

    public bool IsScrolling => scrolling;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (!scrolling) return;

        transform.position += new Vector3(-speed * Time.deltaTime, 0f, 0f);
    }

    public void SetScrolling(bool value)
    {
        scrolling = value;
    }
}
