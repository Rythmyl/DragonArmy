using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("---Potion Settings---")]
    [SerializeField] int healAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        towerHealth tower = other.GetComponent<towerHealth>();
        if (tower != null)
        {
            tower.SetHealth(tower.currentHealth + healAmount);
            Destroy(gameObject);
        }
    }
}
