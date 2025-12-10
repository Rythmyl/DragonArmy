using System.Collections;
using UnityEngine;

public class dragonSpawner : MonoBehaviour
{
    [System.Serializable]
    public class Spawner
    {
        public GameObject dragonPrefab;
        public Transform[] spawnPoints;
        public GameObject towerTarget;
        public int quantityToSpawn;
        public float spawnFrequency;

        [HideInInspector] public int dragonsSpawned = 0;
        [HideInInspector] public int nextSpawnIndex = 0;
    }

    public Spawner[] spawners = new Spawner[4];
    public float delayBetweenSpawners = 30f;

    void Start()
    {
        StartCoroutine(SpawnSpawnersSequentially());
    }

    private IEnumerator SpawnSpawnersSequentially()
    {
        int startIndex = Random.Range(0, spawners.Length);

        for (int i = 0; i < spawners.Length; i++)
        {
            int spawnerIndex = (startIndex + i) % spawners.Length;
            var spawner = spawners[spawnerIndex];

            spawner.dragonsSpawned = 0;
            spawner.nextSpawnIndex = 0;

            gamemanager.instance?.updateGameGoal(spawner.quantityToSpawn, isDragon: true);

            yield return StartCoroutine(SpawnFromSpawner(spawner));

            float spawnDuration = spawner.quantityToSpawn * spawner.spawnFrequency;
            float waitTimeAfterSpawn = delayBetweenSpawners - spawnDuration;
            if (waitTimeAfterSpawn < 0) waitTimeAfterSpawn = 0;

            yield return new WaitForSeconds(waitTimeAfterSpawn);
        }
    }

    private IEnumerator SpawnFromSpawner(Spawner spawner)
    {
        while (spawner.dragonsSpawned < spawner.quantityToSpawn)
        {
            SpawnSingleDragon(spawner);
            spawner.dragonsSpawned++;
            yield return new WaitForSeconds(spawner.spawnFrequency);
        }
    }

    private void SpawnSingleDragon(Spawner spawner)
    {
        if (spawner.spawnPoints.Length == 0) return;

        Transform spawnPoint = spawner.spawnPoints[spawner.nextSpawnIndex];
        spawner.nextSpawnIndex = (spawner.nextSpawnIndex + 1) % spawner.spawnPoints.Length;

        GameObject dragon = Instantiate(spawner.dragonPrefab, spawnPoint.position, spawnPoint.rotation);
        dragon.GetComponent<dragonAI>()?.SetTowerTarget(spawner.towerTarget);
    }
}