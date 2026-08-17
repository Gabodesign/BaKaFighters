using UnityEngine;

public class EnemyActivation : MonoBehaviour
{
    [Header("Target (Player)")]
    [SerializeField] private Transform target;

    [Header("Activation Distance")]
    [SerializeField] private float activationDistance = 22f;
    [SerializeField] private bool controlShooter = true;
    [SerializeField] private bool controlMovement = true;

    [Header("Componenti da controllare")]
    [SerializeField] private Enemy enemy;
    [SerializeField] private EnemyShoot enemyShoot;

    private float sqrActivationDistance;

    private void Awake()
    {
        if (controlMovement && enemy == null)
        {
            enemy = GetComponent<Enemy>();
            if (enemy == null) Debug.LogWarning($"{name}: controlMovement attivo ma manca il componente Enemy!");
        }

        if (controlShooter && enemyShoot == null)
        {
            enemyShoot = GetComponent<EnemyShoot>();
            if (enemyShoot == null) Debug.LogWarning($"{name}: controlShooter attivo ma manca il componente EnemyShoot!");
        }

        sqrActivationDistance = activationDistance * activationDistance;
    }

    private void Update()
    {
        if (target == null) return;

        float sqrDist = (transform.position - target.position).sqrMagnitude;
        bool shouldActivate = sqrDist <= sqrActivationDistance;

        if (controlMovement && enemy != null) enemy.enabled = shouldActivate;
        if (controlShooter && enemyShoot != null) enemyShoot.enabled = shouldActivate;
    }
}