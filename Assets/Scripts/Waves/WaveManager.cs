using System;
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
    public int lastRandomizeWaves = 5;
    private int _currentWaveIndex = 0;
    private int enemiesRemaining;

    public float XPPowerFactor;
    private int currentXPIndex = 0;
    private float accumulatedXP = 0;

    public int currentWaveIndex
	{
		get { return _currentWaveIndex; }
	}

    void Start()
    {
        StartCoroutine(PlayWave(waves[_currentWaveIndex]));
    }

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

        // Check if more spawners are needed and update them
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

        // Do the spawning of the current wave
        int spawnCount = 0;
        while(spawnCount < wave.totalToSpawn)
        {
            // Spawn from all spawners unless limit is reached
            for (int i = 0; i < wave.spawnersInWave.Count(); i++)
            {
                spawners[i].SpawnWeightedEnemy();

                // TO DO: may change if multi spawners
                // TO DO: Can be removed or kept
                enemiesRemaining++;

                spawnCount++;
                if(spawnCount >= wave.totalToSpawn)
                    break;
            }

            yield return new WaitForSeconds(wave.spawnRate);
        }

        // TO DO: Change this for wait seconds. Add it as a wave property
        // Wait until the player clears the board
        //while (enemiesRemaining > 0) yield return null;
        yield return new WaitForSeconds(3);

        EndWave();
    }

    public void updateSpawnerPosition(EnemySpawner spawner)
    {
        Vector3 randomPos = UnityEngine.Random.insideUnitSphere * spawnRadius;
        randomPos += transform.position;
        randomPos.y = 0f;
            
        Vector3 direction = randomPos - transform.position;
        direction.Normalize();
            
        float dotProduct = Vector3.Dot(transform.forward, direction);
        float dotProductAngle = Mathf.Acos(dotProduct / transform.forward.magnitude * direction.magnitude);
            
        randomPos.x = Mathf.Cos(dotProductAngle) * spawnRadius + transform.position.x;
        randomPos.z = Mathf.Sin(dotProductAngle * (UnityEngine.Random.value > 0.5f ? 1f : -1f)) * spawnRadius + transform.position.z;
        
        spawner.gameObject.transform.position = randomPos;
        Debug.Log("Update spawner position "+spawner.spawnerData.spawnerName);
    }
    public void EnemyDied(EnemyController enemy)
    {
        enemiesRemaining--;

        // Add XP
        accumulatedXP += enemy.data.xp;
        CheckXPLevelUp();
    }

    void EndWave()
    {
        // Trigger Rogue-like Power-up UI here!
        Debug.Log("Wave "+waves[_currentWaveIndex].waveName!+" cleared!");

        _currentWaveIndex++;
        
        if (_currentWaveIndex >= waves.Count)
        {
            // We reached the end of the waves, let's recycle among the last 5
            int randomWave = UnityEngine.Random.Range(Math.Max(waves.Count - 1 - lastRandomizeWaves,0),waves.Count - 1);

            Debug.Log("Final wave cleared! Randomizing to "+randomWave);

            _currentWaveIndex = randomWave;
        }

        StartCoroutine(PlayWave(waves[_currentWaveIndex]));
    }

    void CheckXPLevelUp()
    {
        // If level up needed show upgrades
        if(accumulatedXP > Math.Pow(currentXPIndex, XPPowerFactor))
        {

            Debug.Log("Level up! XP level "+(currentXPIndex+1)+", XP amount "+accumulatedXP);

            currentXPIndex++;
            ShowUpgrades();
        }
    }

    void ShowUpgrades()
    {
        Debug.Log("Level up! Pick an upgrade.");
        // TO DO: Stop time
        // TO DO: Wait for player to choose
        ContinueAfterUpgrading();
    }

    void ContinueAfterUpgrading()
    {
        Debug.Log("Upgrade chosen.");
        // TO DO: Resume time

        CheckXPLevelUp();
    }
}