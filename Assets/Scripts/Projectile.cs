using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    public GameObject hitEffectPrefab;
    int damage;

    public void Init(int dmg)
    {
        damage = dmg;
        Destroy(gameObject, 3f);
    }

    private void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Tower"))
        {
            return;
        }

        IDamage dmg = other.GetComponent<IDamage>();

        if (dmg != null)
        {
            dmg.takeDamage(damage);

            if (hitEffectPrefab != null)
            {
                Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
