using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform[] targets;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int numOfWaves;
    [SerializeField] private float timeBetweenWaves = 5f;
    [SerializeField] private int numOfEnemiesPerWave;

    float timeSinceLastSpawn = 0f;
    Transform playerTransform;

    struct SpawnPos
    {
        public Vector3 pos;
        public Transform target;
    }

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        timeSinceLastSpawn = timeBetweenWaves;
    }

    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;

        if (timeSinceLastSpawn > timeBetweenWaves)
        {
            StartCoroutine(SpawnWave());
        }
    }

    // Spawn waves of enemies based on ratios which dynamically change after every level
    private IEnumerator SpawnWave()
    {
        //First scatter some positions 10f behind the player
        yield return null;
        timeSinceLastSpawn = 0;
        int enemiesPerTarget = Mathf.FloorToInt(numOfEnemiesPerWave / targets.Length);
        SpawnPos[] spawnPositions = new SpawnPos[numOfEnemiesPerWave];

        float spawnZPosition = playerTransform.position.z - 10f;
        int spawnPositionsCounter = 0;
        foreach (var target in targets)
        {
            for (int i = 0; i < enemiesPerTarget; i++)
            {
                spawnPositions[spawnPositionsCounter].pos = new Vector3(target.position.x, 0, spawnZPosition);
                spawnPositions[spawnPositionsCounter].target = target;
                spawnPositionsCounter++;
            }
        }

        //Get the ratios
        float level0enemies = Mathf.Floor(gameManager.enemyRatios[0] * numOfEnemiesPerWave);
        float level1enemies = Mathf.Floor(gameManager.enemyRatios[1] * numOfEnemiesPerWave);
        float level2enemies = Mathf.Floor(gameManager.enemyRatios[2] * numOfEnemiesPerWave);
        float level3enemies = Mathf.Floor(gameManager.enemyRatios[3] * numOfEnemiesPerWave);
        int spawnCount = 0;
        GameObject go;
        WaitForSeconds wait = new WaitForSeconds(0.03f);

        //Create enemies
        foreach (var pos in spawnPositions)
        {
            if (level0enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[0], pos.pos, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = pos.target;
                level0enemies--;
                yield return wait;
            }
            else if (level1enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[1], pos.pos, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = pos.target;
                level1enemies--;
                yield return wait;
            }
            else if (level2enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[2], pos.pos, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = pos.target;
                level2enemies--;
                yield return wait;
            }
            else if (level3enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[3], pos.pos, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = playerTransform;
                level3enemies--;
                yield return wait;
            }
            spawnCount++;
        }

        //If some enmies are left unspawn, due to Math.Floor in ratios, spawn them here.
        int lastToSpawn = numOfEnemiesPerWave - spawnCount;
        if (lastToSpawn > 0)
        {
            Vector3 spawnPos = new Vector3(0, 0, spawnZPosition);
            for (int i = 0; i < lastToSpawn; i++)
            {
                go = Instantiate(enemyPrefabs[0], spawnPos, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
                yield return wait;
            }
        }
    }

    public void StopSpawningEnemies()
    {
        timeBetweenWaves = Mathf.Infinity;
    }

}
