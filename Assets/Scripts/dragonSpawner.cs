using UnityEngine;

public class dragonSpawner : MonoBehaviour
{
    [Header("----- Spawn Settings -----")]
    [Range(1, 100)][SerializeField] int spawnAmount;
    [Range(0, 10)][SerializeField] int spawnRate;

    [Header("----- Spawn Objects -----")]
    [SerializeField] GameObject objectToSpawn;

    [Header("----- Spawn Positions -----")]
    [SerializeField] Transform[] spawnPos;

    [Header("----- Tower Reference -----")]
    [SerializeField] GameObject tower;

    int spawnCount;
    float spawnTimer;
    bool startSpawning;

    void Start()
    {
        gamemanager.instance.updateGameGoal(spawnAmount, isDragon: true);
    }

    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
            {
                spawn();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rythmyl"))
        {
            startSpawning = true;
        }
    }

    void spawn()
    {
        if (spawnPos == null || spawnPos.Length == 0)
        {
            return;
        }

        int index = Random.Range(0, spawnPos.Length);
        GameObject spawnedDragon = Instantiate(objectToSpawn, spawnPos[index].position, Quaternion.identity);

        dragonAI dragonScript = spawnedDragon.GetComponent<dragonAI>();
        if (dragonScript != null && tower != null)
        {
            dragonScript.tower = tower;
        }

        spawnCount++;
        spawnTimer = 0;
    }
}
