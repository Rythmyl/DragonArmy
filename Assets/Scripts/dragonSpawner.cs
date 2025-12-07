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
        public int quantityToSpawn = 10;
        public float spawnFrequency = 2f;

        [HideInInspector] public int dragonsSpawned = 0;
        [HideInInspector] public int nextSpawnIndex = 0;
    }

    public Spawner[] spawners;
    public float delayBetweenSpawners = 30f;

    private void Start()
    {
        StartCoroutine(SpawnSpawnersSequentially());
    }

    private IEnumerator SpawnSpawnersSequentially()
    {
        foreach (var spawner in spawners)
        {
            spawner.dragonsSpawned = 0;
            spawner.nextSpawnIndex = 0;

            gamemanager.instance?.updateGameGoal(spawner.quantityToSpawn, isDragon: true);

            float spawnDuration = spawner.quantityToSpawn * spawner.spawnFrequency;
            float waitTimeAfterSpawn = delayBetweenSpawners - spawnDuration;
            if (waitTimeAfterSpawn < 0) waitTimeAfterSpawn = 0;

            yield return StartCoroutine(SpawnFromSpawner(spawner));
            yield return new WaitForSeconds(waitTimeAfterSpawn);
        }
    }

    private IEnumerator SpawnFromSpawner(Spawner spawner)
    {
        while (spawner.dragonsSpawned < spawner.quantityToSpawn)
        {
            Transform spawnPoint = spawner.spawnPoints[spawner.nextSpawnIndex];
            spawner.nextSpawnIndex = (spawner.nextSpawnIndex + 1) % spawner.spawnPoints.Length;

            GameObject dragon = Instantiate(spawner.dragonPrefab, spawnPoint.position, spawnPoint.rotation);
            spawner.dragonsSpawned++;

            var dragonAIComp = dragon.GetComponent<dragonAI>();
            if (dragonAIComp != null && spawner.towerTarget != null)
                dragonAIComp.SetTowerTarget(spawner.towerTarget);

            yield return new WaitForSeconds(spawner.spawnFrequency);
        }
    }
}