using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public SpawnerData spawnerData;
    public GameObject enemiesObject;
    public GameObject xpObject;
    public WaveManager waveManager;
    // DEGUB
     //InputAction debugAction;

     public void Start()
    {
        // DEBUG
        //debugAction = InputSystem.actions.FindAction("Spawn");
    }

    public void SpawnWeightedEnemy()
    {
        if(spawnerData == null)
        {
            //Debug.LogError("Spawner missing Spawner data!");
            return;
        }
        if(spawnerData.enemiesInWave.Count() != spawnerData.enemyWeights.Count())
        {
            //Debug.LogError("Spawner " + spawnerData.spawnerName + " has a mismatch is enemy types and waves. They should be the same size!");
            return;
        }

        // 1. Calculate Total Weight
        int totalWeight = 0;
        foreach (int weight in spawnerData.enemyWeights)
        {
            totalWeight += weight;
        }

        // 2. Pick a random "ticket" number
        int randomNumber = Random.Range(0, totalWeight);
        
        // 3. Find which enemy owns that ticket
        int currentWeightSum = 0;
        EnemyData selectedData = null;

        foreach (var weight in spawnerData.enemyWeights)
        for (int i = 0; i < spawnerData.enemyWeights.Count(); i++)
        {
            currentWeightSum += spawnerData.enemyWeights[i];
            if (randomNumber < currentWeightSum)
            {
                selectedData = spawnerData.enemiesInWave[i];
                break;
            }
        }

        // 4. Instantiate (same as before)
        if (selectedData != null)
        {
            GameObject newEnemy = Instantiate(selectedData.visualPrefab, transform.position, Quaternion.identity, enemiesObject.transform);
            // Set the data
            EnemyController enemyController = newEnemy.GetComponent<EnemyController>();
            if (enemyController != null)
            {
                enemyController.data = selectedData;
                // Update / set the spawner reference
                enemyController.xpObject = xpObject;
                enemyController.enemySpawner = this;
                enemyController.playerRef = waveManager.playerRef;
                waveManager.playerRef.onModeSwitch.AddListener(enemyController.ModeSwitchEnemy);
            }
            else
            {
                //Debug.LogError("Wrong prefab set to an Enemy Spawner, the prefab needs to have an EnemyController");
            }
        }
    }

    // Quick test: Press Space to spawn
    void Update()
    {
        // DEBUG
        //if (debugAction.WasPressedThisFrame()) SpawnWeightedEnemy();
    }
    public void EnemyDied(EnemyController enemy)
    {
        // Notify death
        if(waveManager != null)
        {
            waveManager.EnemyDied(enemy);
        }else
        {
            //Debug.LogError("Wave Manager reference not set to an enemy of type "+spawnerData.name+"! Please, set it up correctly");
        }
    }

    public void Respawn(EnemyController enemy)
    {
        // TO DO: Notify Wave Manager moving needed
        waveManager.updateSpawnerPosition(this);

        // TO DO: Respawn enemy
        enemy.transform.position = transform.position;
    }
}