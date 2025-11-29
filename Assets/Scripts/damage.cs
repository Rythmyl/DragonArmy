using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class damage : MonoBehaviour
{
    enum damageType { moving, stationary, DOT, homing }

    [Header("----- Damage Settings -----")]
    [SerializeField] damageType type;

    [Range(1, 1000)][SerializeField] int damageAmount;
    [Range(0.01f, 5f)][SerializeField] float damageRate;
    [Range(0, 50)][SerializeField] int speed;
    [Range(1, 30)][SerializeField] int destroyTime;

    [Header("----- Components -----")]
    [SerializeField] Rigidbody rb;

    private bool isDamaging;
    private HashSet<IDamage> damagedTargets = new HashSet<IDamage>();

    void Start()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject, destroyTime);

            if (type == damageType.moving && rb != null)
            {
                rb.linearVelocity = transform.forward * speed;
            }
        }
    }

    void Update()
    {
        if (type == damageType.homing && rb != null)
        {
            if (gamemanager.instance != null && gamemanager.instance.rythmyl != null)
            {
                Vector3 direction = (gamemanager.instance.rythmyl.transform.position - transform.position).normalized;
                rb.linearVelocity = direction * speed;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type != damageType.DOT)
        {
            if (other.CompareTag("Rythmyl") || dmg != null)
            {
                dmg.takeDamage(damageAmount);
            }
        }

        if (type == damageType.moving || type == damageType.homing)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.isTrigger)
            return;

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null && type == damageType.DOT && !damagedTargets.Contains(dmg))
        {
            StartCoroutine(DamageOverTime(dmg));
        }
    }

    IEnumerator DamageOverTime(IDamage target)
    {
        damagedTargets.Add(target);
        while (true)
        {
            target.takeDamage(damageAmount);
            yield return new WaitForSeconds(damageRate);
            // Optionally, add a condition to break this loop if needed
        }
        // damagedTargets.Remove(target); // If you add a break condition, remove target here
    }
}
