using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Pool of Enemy Varieties")]
    public List<EnemyData> enemyPool; 
    
    [Header("The Base Enemy Template")]
    public GameObject enemyPrefab; // A prefab with the EnemyController script on it

    public void SpawnRandomEnemy()
    {
        if (enemyPool.Count == 0) return;

        // 1. Pick a random data asset
        int randomIndex = Random.Range(0, enemyPool.Count);
        EnemyData selectedData = enemyPool[randomIndex];

        // 2. Spawn the generic enemy prefab
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // 3. Inject the data into the enemy's controller
        EnemyController controller = newEnemy.GetComponent<EnemyController>();
        if (controller != null)
        {
            controller.data = selectedData;
        }
    }

    // Quick test: Press Space to spawn
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) SpawnRandomEnemy();
    }
}