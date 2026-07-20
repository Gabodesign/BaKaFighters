using Unity.VisualScripting;
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
    
    private void Awake()
    {
        if(controlMovement && enemy == null) enemy = GetComponent<Enemy>();

        if(controlShooter && enemyShoot == null) enemyShoot = GetComponent<EnemyShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;
        float distPlayer = Vector3.SqrMagnitude(transform.position - target.position);
        //Debug.Log(distPlayer);
        if (distPlayer <= activationDistance)
        {
            Debug.Log("Enemy activated: " + gameObject.name);
            // Activate enemy behavior
            if (controlMovement) enemy.enabled = true;
            if (controlShooter) enemyShoot.enabled = true;
        }
        else 
        {
            if (controlMovement) enemy.enabled = false;
            if (controlShooter) enemyShoot.enabled = false; 
        }
    }
}
