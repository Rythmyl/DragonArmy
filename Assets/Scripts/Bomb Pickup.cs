using UnityEngine;

public class BombPickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BombSpawner spawner = other.GetComponent<BombSpawner>();

        if (spawner != null)
        {
            spawner.AddBomb();
            Destroy(gameObject);
        }
    }

   

}
