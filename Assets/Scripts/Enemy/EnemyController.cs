using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public EnemyData data; // Drag your ScriptableObject here in the Inspector

    void Start()
    {
        InitializeEnemy();
    }

    void InitializeEnemy()
    {
        // Set up the enemy based on the data
        Debug.Log($"Spawned {data.enemyName} with {data.health} HP");
        
        // Instantiate the visual model as a child of this object
        Instantiate(data.visualPrefab, transform);
    }

    void Update()
    {
        
    }
}
