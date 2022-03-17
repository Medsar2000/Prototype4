using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] enemyPrefab;

    public GameObject[] powerupPrefab;
    public int enemiesAliveCount;
    public int waveNumber = 1;
    private float spawnRange = 9.0f;

    public GameObject bossPrefab;
    public GameObject[] miniEnemyPrefabs;
    public int bossRound;
    // Start is called before the first frame update
    void Start()
    {
        SpawnEnemyWave(waveNumber);
        SpawnPowerup();

    }

    // Update is called once per frame
    void Update()
    {
        enemiesAliveCount = FindObjectsOfType<Enemy>().Length;
        if (enemiesAliveCount == 0)
        {
            waveNumber++;
            //Spawn a boss every x number of waves
            if (waveNumber % bossRound == 0)
            {
                SpawnBossWave(waveNumber);
            }
            else
            {
                SpawnEnemyWave(waveNumber);
                SpawnPowerup();
            }
        }


    }
    Vector3 GenerateSpawnPosition()
    {
        float spawnX = Random.Range(-spawnRange, spawnRange);
        float spawnZ = Random.Range(-spawnRange, spawnRange);
        Vector3 spawnPos = new Vector3(spawnX, 0, spawnZ);
        return spawnPos;
    }

    private void SpawnEnemyWave(int count)
    {
        for (int i = 0; i < count; i++)
        {
            int index = Random.Range(0, enemyPrefab.Length);
            Instantiate(enemyPrefab[index], GenerateSpawnPosition(), enemyPrefab[index].transform.rotation);

        }
    }
    private void SpawnPowerup()
    {
        int index = Random.Range(0, powerupPrefab.Length);
        Instantiate(powerupPrefab[index], GenerateSpawnPosition(), powerupPrefab[index].transform.rotation);
    }

    void SpawnBossWave(int currentRound)
    {
        int miniEnemysToSpawn;
        //We dont want to divide by 0!
        if (bossRound != 0)
        {
            miniEnemysToSpawn = currentRound / bossRound;
        }
        else
        {
            miniEnemysToSpawn = 1;
        }
        var boss = Instantiate(bossPrefab, GenerateSpawnPosition(), bossPrefab.transform.rotation);
        boss.GetComponent<Enemy>().miniEnemySpawnCount = miniEnemysToSpawn;
    }

    public void SpawnMiniEnemy(int amount) 
    { 
        for (int i = 0; i < amount; i++)
        { 
            int randomMini = Random.Range(0, miniEnemyPrefabs.Length);
            Instantiate(miniEnemyPrefabs[randomMini], GenerateSpawnPosition(), miniEnemyPrefabs[randomMini].transform.rotation); 
        } 
    }
}
