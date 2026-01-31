using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves; // Drag your Wave assets here
    public EnemySpawner spawner; // Reference to your previous spawner
    
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
        
        for (int i = 0; i < wave.totalToSpawn; i++)
        {
            // Pick a random enemy from this specific wave's pool
            EnemyData data = wave.enemiesInWave[Random.Range(0, wave.enemiesInWave.Length)];
            //spawner.SpawnEnemy(data); 
            
            enemiesRemaining++;
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