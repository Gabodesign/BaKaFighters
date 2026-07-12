using UnityEngine;


[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapon/WeaponData")]
public class WeaponData : ScriptableObject
{
    [Header("Weapon Type")]
    public WeaponType WeaponType;

    [Header("Weapon Information")]
    public string weaponName;
    public Sprite icon;
    public WeaponLevelStats[] levels = new WeaponLevelStats[3];
}

[System.Serializable]
public struct WeaponLevelStats
{
    public float damage;
    public float fireRate;
    public float projectileSpeed;
    public int projectilesPerShot;
    public float spreadAngle;
    public float maxDistance;
    public GameObject projectilePrefab;
}

