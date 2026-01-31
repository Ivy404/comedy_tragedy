using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves; // Wave data objects
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public GameObject baseSpawner;  // Reference to get basic spawner
    
    private int _currentWaveIndex = 0;
    private int enemiesRemaining;
    public int currentWaveIndex
	{
		get { return _currentWaveIndex; }
	}

    void Start() => StartCoroutine(PlayWave(waves[_currentWaveIndex]));

    IEnumerator PlayWave(WaveData wave)
    {
        Debug.Log($"Starting {wave.waveName}!");

        for (int i = 0; i < wave.spawnersInWave.Count(); i++)
        {
            //GameObject spawnerObject;
            if(spawners.Count() - 1 < i)
            {
                // we need to instantiate a new one
                GameObject spawnerObject = Instantiate(baseSpawner, transform.position, Quaternion.identity);
                spawners.Add(spawnerObject.GetComponent<EnemySpawner>());
            }
            EnemySpawner spawner = spawners[i];
            if (spawner != null)
            {
                spawner.spawnerData = wave.spawnersInWave[i];
            }
            spawner.waveManager = this;
            // Move spawner
            // TO DO: select a random spot around the wave manager
            // TO DO: update the spawner transform
        }

        int spawnCount = 0;

        while(spawnCount < wave.totalToSpawn)
        {
            // Spawn from all spawners unless limit is reached
            for (int i = 0; i < wave.spawnersInWave.Count(); i++)
            {
                spawners[i].SpawnWeightedEnemy();

                // TO DO: may change if multi spawners
                enemiesRemaining++;

                spawnCount++;
                if(spawnCount >= wave.totalToSpawn)
                    break;
            }

            yield return new WaitForSeconds(wave.spawnRate);
        }

        // Wait until the player clears the board
        while (enemiesRemaining > 0) yield return null;

        EndWave();
    }

    public void EnemyDied()
    {
        enemiesRemaining--;
    }

    void EndWave()
    {
        _currentWaveIndex++;
        if (_currentWaveIndex < waves.Count)
        {
            // Trigger Rogue-like Power-up UI here!
            Debug.Log("Wave Clear! Pick an upgrade.");
            StartCoroutine(PlayWave(waves[_currentWaveIndex]));
        }
        else
        {
            Debug.Log("All waves cleared! You win!");
        }
    }
}