using System.Collections;
using UnityEngine;

public class dragonSpawner : MonoBehaviour
{
    [Header("----- Spawn Settings -----")]
    [Range(1, 100)][SerializeField] int spawnAmount;

    [Range(0.1f, 10f)][SerializeField] float spawnRate;

    [Header("----- Spawn Object (Single dragon prefab) -----")]
    [SerializeField] GameObject dragonPrefab;

    [Header("----- Spawn Positions -----")]
    [SerializeField] Transform[] spawnPos;

    [Header("----- Tower Reference -----")]
    [SerializeField] GameObject tower;

    [Header("----- Dragon Spawners Group -----")]
    [SerializeField] dragonSpawner[] allSpawners;

    int spawnCount;
    bool startSpawning;
    bool isSpawningComplete;

    void Start()
    {
        gamemanager.instance.updateGameGoal(spawnAmount, isDragon: true);

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
            StartSpawningCoroutine();
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
        GameObject dragon = Instantiate(dragonPrefab, spawnPos[index].position, Quaternion.identity);

        dragonAI dragonAIComp = dragon.GetComponent<dragonAI>();
        if (dragonAIComp != null && tower != null)
            dragonAIComp.SetTowerTarget(tower);
    }
}
