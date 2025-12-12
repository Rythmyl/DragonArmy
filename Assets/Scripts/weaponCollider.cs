using UnityEngine;

public class weaponCollider : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rythmyl") || other.CompareTag("Tower"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            if (damageable != null)
                damageable.takeDamage(damageAmount);
        }
    }
}
