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

    private int spawnedCount = 0;
    private bool isSpawning = false;
    private float spawnTimer = 0f;

    void Start()
    {
        gamemanager.instance.updateGameGoal(spawnAmount, isDragon: true);
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
    }
}
