using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.EditorTools;
using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    
    [SerializeField] private EnemiesConfig config;

    [Header("Origine colpo")]
    [SerializeField] private Transform firePoint;
    public enum AimMode
    {
        Forward,            // +X locale (destra)
        Backward,           // −X locale (sinistra)
        Top,                // +Y locale (su)
        Bottom,             // −Y locale (giù)
        DoubleUpDown,       // due colpi: +Y e −Y (su e giù)
        DoubleLeftRight,    // due colpi: +X e −X (destra e sinistra)
        TripleShot,         // tre Colpi: -X, +Y, +X (sinistra/su/destra)
        FourShot,           // quattro colpi: destra/su/sinistra/giù
        AimAtPlayer         // mira verso il Player
    }
    [Header("Mira")]
    [SerializeField] private AimMode aim = AimMode.Forward;
    [SerializeField] private GameObject projectile;
    [SerializeField] private float projSpeed = 10f;
    [SerializeField] private float projDamage = 5f;
    private float nextFireTime = 0f;
    [SerializeField] private float fireRate = 1f;

    [Header("Animazioni d'attacco (Spine)")]
    [SerializeField] private SkeletonAnimation spine;              
    [SerializeField] private string idleAnimName = "";             
    [SerializeField] private string attackAnimName = "";
    [SerializeField] private int spineTrackIndex = 0;
    [SerializeField] private float attackAnimMix = 0.1f;           
    [SerializeField] private bool attackAnimLoop = false;          
    private void Awake()
    {
        if (firePoint == null) firePoint = transform;
    }

    private void OnEnable()
    {
        if (spine != null && !string.IsNullOrEmpty(idleAnimName))
        {
            spine.AnimationState.SetAnimation(spineTrackIndex, idleAnimName, true); // Idle in loop
            spine.AnimationState.TimeScale = 1f;                                     // Velocità normale
        }
    }
    

    // Update is called once per frame
    void Update()
    {
        if (!config.canShot) return;

        if (Time.time < nextFireTime) return;

        if (projectile == null) return;

        var dirs = GetAimDirections();                             
        if (dirs == null || dirs.Count == 0) return;

                                                            
        Shoot(dirs);
        PlayAttackAnimation();
        nextFireTime = Time.time + 1f / fireRate;

    }

    public void Shoot(List<Vector2> dirs)
    {
        int spawned = 0;
        for (int i = 0; i < dirs.Count; i++)
        {
            Vector2 dir = dirs[i];                                          // Direzione corrente (già normalizzata)
            if (dir.sqrMagnitude < 0.0001f) continue;                       // Se nulla, salta

            float ang = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;          // Direzione → angolo
            Quaternion rot = Quaternion.Euler(0f, 0f, ang);                 // Rotazione per il proiettile

            Vector3 pos = firePoint != null ? firePoint.position : transform.position; // Posizione di spawn
            GameObject projectileEnemy = Instantiate(projectile, pos, rot);        // Spawna il proiettile

            var proj = projectileEnemy.GetComponent<Projectile2D>();                     // Script Projectile2D
            if (proj != null)
            {
                proj.Launch(dir, projSpeed, projDamage);             // Lancia il proiettile
                Destroy(projectileEnemy, proj.maxLifetime);
            }

            spawned++;                                                      // Contatore ++

        }
    }

    List<Vector2> GetAimDirections()
    {
        var list = new List<Vector2>();

        // Determina gli assi locali
        Vector2 right, up;
        if (firePoint != null)
        {
            right = firePoint.right;
            up = firePoint.up;
        }
        else
        {
            return list;
        }

        switch (aim)
        {
            case AimMode.Forward:
                list.Add(right.normalized);
                break;

            case AimMode.Backward:
                list.Add((-right).normalized);
                break;

            case AimMode.Top:
                list.Add(up.normalized);
                break;

            case AimMode.Bottom:
                list.Add((-up).normalized);
                break;

            case AimMode.DoubleUpDown:
                list.Add(up.normalized);
                list.Add((-up).normalized);
                break;

            case AimMode.DoubleLeftRight:
                list.Add(right.normalized);
                list.Add((-right).normalized);
                break;

            case AimMode.TripleShot:
                list.Add(right.normalized);
                list.Add(up.normalized);
                list.Add((-right).normalized);
                break;

            case AimMode.FourShot:
                list.Add(right.normalized);
                list.Add(up.normalized);
                list.Add((-right).normalized);
                list.Add((-up).normalized);
                break;

            case AimMode.AimAtPlayer:
                break;
        }

        return list;
    }
    void PlayAttackAnimation()
    {
        // Spine
        if (spine != null && !string.IsNullOrEmpty(attackAnimName))
        {
            var state = spine.AnimationState;
            var attackEntry = state.SetAnimation(spineTrackIndex, attackAnimName, attackAnimLoop);

            if (attackEntry != null && attackAnimMix > 0f)
                attackEntry.MixDuration = attackAnimMix;

            if (!attackAnimLoop && !string.IsNullOrEmpty(idleAnimName))
            {
                var idleEntry = state.AddAnimation(spineTrackIndex, idleAnimName, true, 0f);
                if (idleEntry != null && attackAnimMix > 0f)
                    idleEntry.MixDuration = attackAnimMix;
            }
        }
    }
    }
