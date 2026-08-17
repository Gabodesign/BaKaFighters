using UnityEngine;

public class Deathfly : Enemy
{
    private Vector2 moveDirection;
    public override void Start()
    {
        base.Start(); // Fondamentale per impostare currentHealth
    }

    void Update()
    {
        // Accediamo a canMove tramite il reference "data" ereditato da Enemy
        if (!data.canMove) return;

        SelectDirection(dir);
    }

    void SelectDirection(DIRECTION direction)
    {


        switch (direction)
        {
            case DIRECTION.Forward:
                moveDirection = new Vector2(-1f, 0f);
                break;
            default:
                moveDirection = Vector2.zero;
                break;
        }

        if (isNormalized)
        {
            moveDirection = moveDirection.normalized;
        }

        if (movement != null)
        {
            movement.Move();
        }
        else
        {
            Vector3 delta = (Vector3)(moveDirection * MoveSpeed * Time.deltaTime);
            transform.Translate(delta, Space.World);
        }
    }    
}
