using UnityEngine;

public class BombPickup : MonoBehaviour
{
    private bool playerinRange = false;
    private BombSpawner playerBombSpawner; 


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (playerinRange && Input.GetKeyDown(KeyCode.E))
            {
            Pickup();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();

        if (player != null)
        {
            playerinRange = true;
            playerBombSpawner = other.GetComponent<BombSpawner>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if(other.GetComponent<playerController>() != null)
        {
            playerinRange=false;
        }
    }

    void Pickup()
    {
        if(playerBombSpawner != null)
        {
            playerBombSpawner.AddBomb();
     
        }

        Destroy(gameObject);
    
    }

}
