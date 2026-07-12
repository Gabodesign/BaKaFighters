using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "BaKa/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int maxHealth;
    public float moveSpeed;
    public int scoreValue;
    public int touchDamage;
    public bool useTouchDamage; 
    public bool canMove;
    public bool canShoot;
    public GameObject bulletPrefab;
    public Color colorDamage = Color.red;
}