using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public EnemySpawner enemySpawner;
	private Transform enemyTransform;

    // DEGUB
    InputAction debugAction;

    void Start()
    {
        // cache transform
        enemyTransform = GetComponent<Transform>();

        InitializeEnemy();

        // DEBUG
        debugAction = InputSystem.actions.FindAction("Spawn");
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
        // DEBUG
        if (debugAction.WasPressedThisFrame()) Die();
    }

    void Die()
    {
        // Notify death
        if(enemySpawner != null)
        {
            enemySpawner.EnemyDied(this);
        }else
        {
            Debug.LogError("Enemy Spawner reference not set to an enemy of type "+data.enemyName+"! Please, set it up correctly");
        }

        // TO DO: disable the enemy
        Destroy(gameObject);
    }
}
