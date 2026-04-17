using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponsConfig : MonoBehaviour
{
    public enum WeaponType
    {
        Bullet = 0
    }
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public GameObject projectilePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public virtual void Start()
    {
        
    }   
}
