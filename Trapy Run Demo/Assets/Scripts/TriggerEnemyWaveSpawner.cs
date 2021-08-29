using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnemyWaveSpawner : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Transform[] targets;
    [SerializeField] private Transform spawnTrans;
    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int numOfEnemies;

    bool called = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && called == false)
        {
            called = true;
            StartCoroutine(SpawnWave(other.transform));
        }
    }


    private IEnumerator SpawnWave(Transform playerTrans)
    {
        yield return null;
        float level0enemies = Mathf.Floor(gameManager.enemyRatios[0] * numOfEnemies);
        float level1enemies = Mathf.Floor(gameManager.enemyRatios[1] * numOfEnemies);
        float level2enemies = Mathf.Floor(gameManager.enemyRatios[2] * numOfEnemies);
        float level3enemies = Mathf.Floor(gameManager.enemyRatios[3] * numOfEnemies);
        int spawnCount = 0;
        GameObject go;
        WaitForSeconds wait = new WaitForSeconds(0.03f);
        for (int i = 0; i < numOfEnemies; i++)
        {
            if (level0enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[0], spawnTrans.position, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
                level0enemies--;
                yield return wait;
            }
            else if (level1enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[1], spawnTrans.position, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
                level1enemies--;
                yield return wait;
            }
            else if (level2enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[2], spawnTrans.position, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
                level2enemies--;
                yield return wait;
            }
            else if (level3enemies >= 1)
            {
                go = Instantiate(enemyPrefabs[3], spawnTrans.position, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = playerTrans;
                level3enemies--;
                yield return wait;
            }
            spawnCount++;
        }

        int lastToSpawn = numOfEnemies - spawnCount;
        if (lastToSpawn > 0)
        {
            for (int i = 0; i < lastToSpawn; i++)
            {
                go = Instantiate(enemyPrefabs[0], spawnTrans.position, Quaternion.identity);
                go.GetComponent<Enemy>().moveTarget = targets[UnityEngine.Random.Range(0, targets.Length)];
                yield return wait;
            }
        }
    }
}
