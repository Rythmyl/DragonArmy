using UnityEngine;

public class BombThrower : MonoBehaviour
{
    public Transform firePoint;
    public float throwForce = 10f;
    public GameObject bombPrefab;


    public void ThrowBomb(GameObject bombPrefab)
    {
        GameObject bomb = Instantiate(bombPrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bomb.GetComponent<Rigidbody>();
        rb.AddForce(firePoint.forward * throwForce, ForceMode.Impulse);
    }
}
