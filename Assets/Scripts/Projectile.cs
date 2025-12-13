using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    Transform target;
    int damage;


    public void Init(Transform t, int dmg)
    {
        target = t; 
        damage = dmg;
        Destroy(gameObject, 3f);
    }


    private void Update()
    {
        if(target != null)
        {
            Destroy(gameObject);
            return; 
        }

        transform.position  = Vector3.MoveTowards(transform.position, target.position, speed*Time.deltaTime);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform!=target)
        {
            IDamage dmg = other.GetComponent<IDamage>();
            if(dmg!=null)
            {
                dmg.takeDamage(damage);

                Destroy(gameObject);
            }
        }
    }

}
