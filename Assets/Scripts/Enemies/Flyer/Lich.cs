using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Lich : EnemiesConfig
{
    [SerializeField] private bool isNormalized = true;
    private Vector2 moveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
    }
    void Update()
    {
        if (!canMove) return;

        if (isNormalized)
        {
            moveDirection = moveDirection.normalized;
        }

        SelectDirection(dir);

    }

    void SelectDirection(Enum dir)
    {
        switch (dir)
        {
            case DIRECTION.Forward:
                moveDirection = new Vector2(-1f, 0f);
                break;
            case DIRECTION.Backward:
                moveDirection = new Vector2(1f, 0f);
                break;
            case DIRECTION.Top:
                moveDirection = new Vector2(0f, 1f);
                break;
            case DIRECTION.Bottom:
                moveDirection = new Vector2(0f, -1f);
                break;
        }
        Vector3 delta = (Vector3)(moveDirection * moveSpeed * Time.deltaTime);
        transform.Translate(delta, Space.World);
    }
}
