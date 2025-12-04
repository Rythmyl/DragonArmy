using UnityEngine;

public class towerHealth : MonoBehaviour
{
    [Header("----- Tower Health Settings -----")]

    [Range(1, 1000)][SerializeField] public int maxHealth;

    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            OnTowerDestroyed();
        }
    }

    void OnTowerDestroyed()
    {
        gamemanager.instance.youLose(); 
    }
}
