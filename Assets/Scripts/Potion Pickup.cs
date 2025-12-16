using UnityEngine;

public class PotionPickup : MonoBehaviour
{
    [Header("---Potion Settings---")]
    [SerializeField] int healAmount = 25;

    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();

        if(player != null )
        {
            player.Heal(healAmount);
            Destroy(gameObject);
        }

    }
}
