using UnityEngine;

public class Lich : Enemy
{
    [SerializeField] private bool isNormalized = true;
    private Vector2 moveDirection;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
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
        }

        if (isNormalized)
        {
            moveDirection = moveDirection.normalized;
        }

        // Accediamo a moveSpeed tramite "data"
        Vector3 delta = (Vector3)(moveDirection * data.moveSpeed * Time.deltaTime);
        transform.Translate(delta, Space.World);
    }
}
