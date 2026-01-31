using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;

    void Start()
    {
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        // Set up the enemy based on the data
        Debug.Log($"Spawned {data.enemyName} with {data.health} HP");
        
        // Multiply base stats by the current wave index
        //int waveLevel = FindFirstObjectByType<WaveManager>().currentWaveIndex;
        //float difficultyMultiplier = 1f + (waveLevel * 0.2f); // +20% per wave

        //currentHealth = data.health * difficultyMultiplier;
        // ...
    }

    void Update()
    {
        
    }
}
