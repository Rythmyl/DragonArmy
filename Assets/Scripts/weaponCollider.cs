using UnityEngine;

public class weaponCollider : MonoBehaviour
{
    [SerializeField] private int damageAmount = 5;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"Weapon Collider triggered by: {other.gameObject.name}");

        if (other.CompareTag("Rythmyl"))
        {
            IDamage damageable = other.GetComponent<IDamage>();
            if (damageable != null)
            {
                damageable.takeDamage(damageAmount);
                Debug.Log($"{gameObject.name} hit player for {damageAmount} damage.");
            }
            else
            {
                Debug.LogWarning($"{other.gameObject.name} does not implement IDamage.");
            }
        }
    }
}
