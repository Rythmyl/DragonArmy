using UnityEngine;

public class HealingFlask : MonoBehaviour
{
    [Header("----- Flask Setting -----")]
    [SerializeField] int healAmount = 25;
    [SerializeField] int maxUses = 5;


    int currentUses;

    private void Awake()
    {
        currentUses = maxUses; 
    }

    public void UseFlask(playerController player)
    {
        if(currentUses <= 0)
        {
            return;
        }


        player.Heal(healAmount);
        currentUses--;
    }

    public void Refill()
    {
        currentUses = maxUses;
    }
}

