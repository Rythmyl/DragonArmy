using UnityEngine;
using System;

public class towerHealth : MonoBehaviour, IDamage
{
    [Header("----- Tower Health Settings -----")]
    [Range(1, 1000)][SerializeField] public int maxHealth;
    public int currentHealth;

    public event Action<int> OnHealthChanged;

    void Start()
    {
        currentHealth = maxHealth;
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;
        OnHealthChanged?.Invoke(currentHealth);

        if (currentHealth == 0)
        {
            OnTowerDestroyed();
        }
    }

    public void takeDamage(int amount)
    {
        Debug.Log($"Tower taking damage: {amount}");
        TakeDamage(amount);
    }

    void OnTowerDestroyed()
    {
        gamemanager.instance.youLose();
    }

    private void OnTriggerEnter(Collider other)
    {
        damage damageComponent = other.GetComponent<damage>();
        if (damageComponent != null)
        {
            TakeDamage(damageComponent.damageAmount);
        }
    }

    public void SetHealth(int health)
    {
        currentHealth = Mathf.Clamp(health, 0, maxHealth);
        OnHealthChanged?.Invoke(currentHealth);
    }

    public void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(currentHealth);
    }
}
