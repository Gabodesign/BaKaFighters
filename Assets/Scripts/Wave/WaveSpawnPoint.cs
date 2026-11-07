using UnityEngine;

public class WaveSpawnPoint : MonoBehaviour
{
    [Header("Decidiamo la direzione del enemy spawnato")]
    [SerializeField] private bool overrideDirection;
    [SerializeField] private Enemy.DIRECTION direction = Enemy.DIRECTION.Forward;

    [Header("Decidiamo lo sparo del enemy Spawanto")]
    [SerializeField] private bool overrideAim;
    [SerializeField] private EnemyShoot.AimMode aim = EnemyShoot.AimMode.Forward;

    public void ApplyTo(GameObject enemyObject)
    {
        if (overrideDirection && enemyObject.TryGetComponent(out Enemy enemy))
        {
            enemy.dir = direction;
        }

        if (overrideAim && enemyObject.TryGetComponent(out EnemyShoot shoot))
        {
            shoot.SetAim(aim);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        if (!overrideDirection) return;

        // Stesse convenzioni di Enemy: Forward = verso sinistra
        Vector3 dir = direction switch
        {
            Enemy.DIRECTION.Forward => Vector3.left,
            Enemy.DIRECTION.Backward => Vector3.right,
            Enemy.DIRECTION.Top => Vector3.up,
            _ => Vector3.down
        };
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + dir * 1.5f);
        Gizmos.DrawWireSphere(transform.position + dir * 1.5f, 0.15f);
    }

}
