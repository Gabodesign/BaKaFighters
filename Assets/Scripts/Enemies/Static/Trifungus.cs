using UnityEngine;
using UnityEngine.EventSystems;

public class Trifungus : EnemiesConfig
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (!canMove) return;

    }
}
