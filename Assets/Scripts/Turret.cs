using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Turret : MonoBehaviour
{


    [Header("---Detection---")]
    public float range = 8f;
    public LayerMask excludeLayers;

    [Header("---Shooting---")]
    public float fireRate = 1f;
    public int damage = 5;
    public GameObject projectilePrefab;
    public Transform firePoint;

    float fireTimer; 
    List<Transform> targets = new List<Transform>();



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fireTimer += Time.deltaTime;

        if(targets.Count > 0 && fireTimer >= 1f/fireRate)
        {
            Shoot(targets[0]);
            fireTimer = 0f;
        }
    }

    void Shoot(Transform target)
    {
        if (target == null)
            return;


        GameObject proj = Instantiate(projectilePrefab,firePoint.position,Quaternion.identity);
        proj.GetComponent<Projectile>().Init(target, damage);
    }

    private void OnTriggerEnter(Collider other)
    {
        if((excludeLayers.value & (1<<other.gameObject.layer)) != 0)
        {
            return;
        }
        
        targets.Add(other.transform);
    }

    private void OnTriggerExit(Collider other)
    {
        targets.Remove(other.transform);
    }

}
