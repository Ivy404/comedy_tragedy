using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemySpawner : MonoBehaviour
{
    public SpawnerData spawnerData;
    
    // DEGUB
     InputAction debugAction;

     public void Start()
    {
        // DEBUG
        debugAction = InputSystem.actions.FindAction("Spawn");
    }

    public void SpawnWeightedEnemy()
    {
        if(spawnerData == null)
        {
            Debug.LogError("Spawner missing Spawner data!");
            return;
        }
        if(spawnerData.enemiesInWave.Count() != spawnerData.enemyWeights.Count())
        {
            Debug.LogError("Spawner " + spawnerData.spawnerName + " has a mismatch is enemy types and waves. They should be the same size!");
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
            GameObject newEnemy = Instantiate(selectedData.visualPrefab, transform.position, Quaternion.identity);
            // Set the data
            EnemyController controller = newEnemy.GetComponent<EnemyController>();
            if (controller != null)
            {
                controller.data = selectedData;
            }
        }
    }

    // Quick test: Press Space to spawn
    void Update()
    {
        // DEBUG
        if (debugAction.WasPressedThisFrame()) SpawnWeightedEnemy();
    }
}