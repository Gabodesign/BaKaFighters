using System;
using UnityEditor.EditorTools;
using UnityEngine;
public class BulletArm : MonoBehaviour
{
    private WeaponController weaponController;
    
    private float nextFireTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        if(weaponController == null)
        {
            weaponController = GetComponent<WeaponController>();
        }
    }

    private void Update()
    {
        if (weaponController.isShooting)
        {
            WeaponLevelStats stats = weaponController.weaponsData[(int)weaponController.startingWeapon].levels[weaponController.currentWeaponLevel];
            if (Time.time >= nextFireTime)
            {
                nextFireTime = Time.time + 1 / stats.fireRate;
                Shoot(stats);
            }
        }
    }

    void Shoot(WeaponLevelStats stats)
    {
        int count = Mathf.Max(1, stats.projectilesPerShot);
        float spread = stats.spreadAngle;

        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(AudioManager.instance.bullet);
        }

        if (count == 1)
        {
            GameObject proj = Instantiate(stats.projectilePrefab, weaponController.firePoint.position, weaponController.firePoint.rotation);
            var projectile = proj.GetComponent<Projectile2D>();

            if (projectile != null)
            {
                projectile.Launch(weaponController.firePoint.right, stats.projectileSpeed, stats.damage);
                Destroy(proj, projectile.maxLifetime);
            }
            else
            {
                Destroy(proj, 2f);
            }
            return;
        }

        float halfSpread = spread / 2f;

        for (int i = 0; i < count; i++)
        {
            float t = (float)i / (count - 1);

            float angle = Mathf.Lerp(-halfSpread, halfSpread, t);

            Vector2 direction = Quaternion.Euler(0, 0, angle) * weaponController.firePoint.right;

            Quaternion bulletRotation = weaponController.firePoint.rotation * Quaternion.Euler(0, 0, angle);

            GameObject proj = Instantiate(stats.projectilePrefab, weaponController.firePoint.position, bulletRotation);
            var projectile = proj.GetComponent<Projectile2D>();

            if (projectile != null)
            {
                projectile.Launch(direction, stats.projectileSpeed, stats.damage);
                Destroy(proj, projectile.maxLifetime);
            }
            else
            {
                Destroy(proj, 2f);
            }
        }
    }

}