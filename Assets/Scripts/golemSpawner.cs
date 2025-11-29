using UnityEngine;

public class golemSpawner : MonoBehaviour
{
    [Header("----- Spawn Settings -----")]
    [Range(1, 100)] [SerializeField] int spawnAmount;
    [Range(0, 10)] [SerializeField] int spawnRate;

    [Header("----- Spawn Objects -----")]
    [SerializeField] GameObject objectToSpawn;  // Assign Golem prefab here

    [Header("----- Spawn Positions -----")]
    [SerializeField] Transform[] spawnPos;

    int spawnCount;
    float spawnTimer;
    bool startSpawning;

    void Start()
    {
        gamemanager.instance.updateGameGoal(spawnAmount, true, false); // Golem count update
    }

    void Update()
    {
        if (startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if (spawnCount < spawnAmount && spawnTimer >= spawnRate)
            {
                Spawn();
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

    void Spawn()
    {
        if (spawnPos == null || spawnPos.Length == 0)
        {
            Debug.LogError("No spawn positions assigned to golemSpawner!");
            return;
        }

        int index = Random.Range(0, spawnPos.Length);
        Instantiate(objectToSpawn, spawnPos[index].position, Quaternion.identity);
        spawnCount++;
        spawnTimer = 0;
    }
}
