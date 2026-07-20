using UnityEngine;

public class Kozo : Enemy
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

    // Cambiato il tipo dell'argomento da Enum a DIRECTION
    void SelectDirection(DIRECTION direction)
    {
    

        switch (direction)
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