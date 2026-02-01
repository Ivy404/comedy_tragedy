using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public List<WaveData> waves; // Wave data objects
    [SerializeField] private GameObject enemiesObject;
    private List<EnemySpawner> spawners = new List<EnemySpawner>();
    public GameObject baseSpawner;  // Reference to get basic spawner
    public float spawnRadius = 15f;
    public PlayerActions playerRef;
    private int _currentWaveIndex = 0;
    private int enemiesRemaining;
    public int currentWaveIndex
	{
		get { return _currentWaveIndex; }
	}

    void Start() => StartCoroutine(PlayWave(waves[_currentWaveIndex]));

    void Update()
    {
        // Follow the player
        if(playerRef != null)
        {
            gameObject.transform.position = new Vector3(playerRef.transform.position.x, playerRef.transform.position.y, playerRef.transform.position.z);
        }
        else
        {
            Debug.LogError("WaveManager is missing the player reference in the scene!");
        }
    }
    IEnumerator PlayWave(WaveData wave)
    {
        Debug.Log($"Starting {wave.waveName}!");

        for (int i = 0; i < wave.spawnersInWave.Count(); i++)
        {
            //GameObject spawnerObject;
            if(spawners.Count() - 1 < i)
            {
                // we need to instantiate a new one
                GameObject spawnerObject = Instantiate(baseSpawner, transform.position, Quaternion.identity,gameObject.transform);
                spawners.Add(spawnerObject.GetComponent<EnemySpawner>());
            }
            EnemySpawner spawner = spawners[i];
            if (spawner != null)
            {
                spawner.spawnerData = wave.spawnersInWave[i];
            }
            spawner.waveManager = this;
            spawner.enemiesObject = enemiesObject;
            
            // Move spawner
            updateSpawnerPosition(spawner);
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

    public void updateSpawnerPosition(EnemySpawner spawner)
    {
        Vector3 randomPos = Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;
        randomPos.y = 0f;
            
        Vector3 direction = randomPos - transform.position;
        direction.Normalize();
            
        float dotProduct = Vector3.Dot(transform.forward, direction);
        float dotProductAngle = Mathf.Acos(dotProduct / transform.forward.magnitude * direction.magnitude);
            
        randomPos.x = Mathf.Cos(dotProductAngle) * spawnRadius + transform.position.x;
        randomPos.z = Mathf.Sin(dotProductAngle * (Random.value > 0.5f ? 1f : -1f)) * spawnRadius + transform.position.z;
        
        spawner.gameObject.transform.position = randomPos;
        Debug.Log("Update spawner position "+spawner.spawnerData.spawnerName);
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