using System.Collections;
using UnityEngine;
using System.Collections;

public class dragonSpawner : MonoBehaviour
{
    [Header("----- Spawn Settings -----")]
    [Range(1, 100)][SerializeField] int spawnAmount;
<<<<<<< Updated upstream
    [Range(0.1f, 10f)][SerializeField] float spawnRate;
=======

    [Range(0, 10)][SerializeField] int spawnRate;
>>>>>>> Stashed changes

    [Header("----- Spawn Object (Single dragon prefab) -----")]
    [SerializeField] GameObject dragonPrefab;

    [Header("----- Spawn Positions -----")]
    [SerializeField] Transform[] spawnPos;

<<<<<<< Updated upstream
    [Header("----- Tower Reference -----")]
    [SerializeField] GameObject tower;

    private int spawnedCount = 0;
    private bool isSpawning = false;
    private float spawnTimer = 0f;
=======
    [Header("----- Dragon Spawners Group -----")]
    [SerializeField] dragonSpawner[] allSpawners;

    [Header("----- Target Tower -----")]
    [SerializeField] GameObject tower;

    int spawnCount;
    bool startSpawning;
    bool isSpawningComplete;
>>>>>>> Stashed changes

    void Start()
    {
        gamemanager.instance.updateGameGoal(spawnAmount, isDragon: true);
<<<<<<< Updated upstream
        StartCoroutine(StartSpawningAfterDelay(3f));
    }

    void Update()
    {
        if (!isSpawning)
            return;

        spawnTimer += Time.deltaTime;

        if (spawnedCount < spawnAmount && spawnTimer >= spawnRate)
        {
            SpawnDragon();
            spawnTimer = 0f;
        }
        else if (spawnedCount >= spawnAmount)
        {
            isSpawning = false;
        }
    }

    void SpawnDragon()
    {
        int spawnIndex = Random.Range(0, spawnPos.Length);
        GameObject dragon = Instantiate(dragonPrefab, spawnPos[spawnIndex].position, Quaternion.identity);
        var ai = dragon.GetComponent<dragonAI>();
        if (ai != null)
            ai.tower = tower;
        spawnedCount++;
    }

    System.Collections.IEnumerator StartSpawningAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        spawnedCount = 0;
        spawnTimer = spawnRate;
        isSpawning = true;
=======

        if (allSpawners != null && allSpawners.Length > 0)
        {
            foreach (var spawner in allSpawners)
                spawner.enabled = false;

            int randomIndex = Random.Range(0, allSpawners.Length);
            allSpawners[randomIndex].enabled = true;
            allSpawners[randomIndex].StartSpawningCoroutine();

            StartCoroutine(SpawnChain(randomIndex));
        }
        else
        {
            startSpawning = true;
        }
    }

    IEnumerator SpawnChain(int startIndex)
    {
        int count = allSpawners.Length;
        for (int i = 1; i < count; i++)
        {
            yield return new WaitForSeconds(10f);
            int nextIndex = (startIndex + i) % count;
            if (allSpawners[nextIndex] != null)
            {
                allSpawners[nextIndex].enabled = true;
                allSpawners[nextIndex].StartSpawningCoroutine();
            }
        }
    }

    public void StartSpawningCoroutine()
    {
        startSpawning = true;
        spawnCount = 0;
        StartCoroutine(SpawnDragons());
    }

    IEnumerator SpawnDragons()
    {
        while (spawnCount < spawnAmount)
        {
            SpawnDragon();
            spawnCount++;
            yield return new WaitForSeconds(spawnRate);
        }
        isSpawningComplete = true;
    }

    void SpawnDragon()
    {
        if (spawnPos == null || spawnPos.Length == 0)
            return;

        int index = Random.Range(0, spawnPos.Length);

        GameObject dragon = Instantiate(objectToSpawn, spawnPos[index].position, Quaternion.identity);

        dragonAI dragonAIComp = dragon.GetComponent<dragonAI>();
        if (dragonAIComp != null && tower != null)
            dragonAIComp.SetTowerTarget(tower);
>>>>>>> Stashed changes
    }
}
