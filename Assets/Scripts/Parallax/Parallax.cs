using UnityEngine;

public class Parallax : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float movex = -1f;
        transform.position += new Vector3(movex * Time.deltaTime, 0f, 0f);
    }
}
