using UnityEngine;
using System;
public class BulletArm : WeaponsConfig
{
    private PlayerController playerController;
    private float nextFireTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public override void Start()
    {
        base.Start();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if (playerController.isShooting)
        {
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1 / fireRate;
                Shoot();
            }
        }
    }

    void Shoot()
    {
        
        GameObject proj = Instantiate(projectilePrefab, playerController.firePoint.position, playerController.firePoint.rotation);
        var projectile = proj.GetComponent<Projectile2D>();
        if(projectile != null)
        {
            projectile.damage = damage;
            Destroy(proj, projectile.maxLifetime);
        }
        else
        {
            Destroy(proj, 2f);
        }
        
    }
}