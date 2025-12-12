using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("-----Bomb-----")]
    public float explosionraidus = 5f;
    public int damage = 10;
    public float fuseTime = 5f;
    public GameObject explosionEffect;

    private void Start()
    {
        Invoke("Explode", fuseTime);
    }

    void Explode()
    {
        if (explosionEffect != null)
            Instantiate(explosionEffect, transform.position, Quaternion.identity);


        Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionraidus);


        foreach(Collider hit in hitColliders)
        {
            IDamage dmg = hit.GetComponent<IDamage>();

            if(dmg != null)
            {
                dmg.takeDamage(damage);
            }


        }

        Destroy(gameObject);          
    
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionraidus);
        
    }


}
