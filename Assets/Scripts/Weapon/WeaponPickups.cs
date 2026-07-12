using UnityEngine;
[RequireComponent(typeof(CircleCollider2D))]
public class WeaponPickups : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            WeaponController weaponController = other.GetComponent<WeaponController>();
            if (weaponController != null)
            {
                weaponController.UpgradeWeapon();   
                Destroy(gameObject); 
            }
        }
    }

}
