using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public EnemySpawner enemySpawner;
	private Transform enemyTransform;
    private NavMeshAgent agent;
    public PlayerActions playerRef;
    private float lastAttackTime;
    private float currentHealth;

    [Header("Movement Settings")]
    public float separationDistance = 0.5f;
    public float separationForce = 5f;
    

    // DEGUB
    InputAction debugAction;

    void Start()
    {
        // cache transform
        enemyTransform = GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        InitializeEnemy();

        // DEBUG
        debugAction = InputSystem.actions.FindAction("Submit");
    }

    void InitializeEnemy()
    {
        if(data == null)
        {
            Debug.LogError("An EnemyController is missing the EnemyData config!");
            return;
        }
        // Set up the enemy based on the data
        Debug.Log($"Spawned {data.enemyName} with {data.health} HP");

        // restore health
        currentHealth = data.health;

        if(agent == null)
        {
            Debug.LogError("An EnemyController is missing the Nav Mesh Agent component!");
            return;
        }
        agent.speed = data.speed;
        agent.stoppingDistance = data.stoppingDistance;
        
        // Multiply base stats by the current wave index
        //int waveLevel = FindFirstObjectByType<WaveManager>().currentWaveIndex;
        //float difficultyMultiplier = 1f + (waveLevel * 0.2f); // +20% per wave

        //currentHealth = data.health * difficultyMultiplier;
        // ...
    }

    void Update()
    {
        if(playerRef != null)
        {
            float distance = Vector3.Distance(transform.position, playerRef.transform.position);
            if (distance > agent.stoppingDistance)
            {
                MoveAndAvoid();
            }else
            {
                 TryAttack();
            }
        }
        // DEBUG
        if (debugAction.WasPressedThisFrame()) Die();
    }

    void MoveAndAvoid()
    {
        // 1. Calculate Chase Vector
        Vector3 directionToPlayer = (playerRef.transform.position - transform.position).normalized;
        
        // 2. Calculate Separation Vector (Mutual Avoidance)
        Vector3 separationVector = Vector3.zero;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, separationDistance);

        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
                // Push away from neighbors
                Vector3 diff = transform.position - col.transform.position;
                separationVector += diff.normalized / diff.magnitude; // Stronger push when closer
            }
        }

        // 3. Combine and Move
        Vector3 finalMove = (directionToPlayer + (separationVector * separationForce)).normalized;
        
        // Ensure they stay on the floor (keep Y consistent)
        finalMove.y = 0;

        // Move towards player
        transform.position += finalMove * data.speed * Time.deltaTime;
        //agent.SetDestination(player.position);
        //agent.SetDestination(transform.position + finalMove * data.speed * Time.deltaTime);
        
        // 4. Rotate to face movement
        if (finalMove != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, finalMove, Time.deltaTime * 10f);
        }
    }

    void TryAttack()
    {
        // Simple attack cooldown using data.attackRate (add this to your ScriptableObject!)
        if (Time.time >= lastAttackTime + 1.5f) 
        {
            Debug.Log($"Attacking player for {data.damage} damage!");

            playerRef.takeDamage(data.damage);
            
            // Add this inside TryAttack()
            // Visual "Bump" toward player
            //transform.position = Vector3.MoveTowards(transform.position, player.position, 0.5f);
            
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(float amount, Vector3 attackerPosition)
    {
        // 1. Calculate the direction from the enemy to the attacker
        Vector3 directionToAttacker = (attackerPosition - transform.position).normalized;

        // 2. Use Dot Product to check the angle
        // If the result is > 0.5, the attacker is roughly in front of the enemy
        float dot = Vector3.Dot(transform.forward, directionToAttacker);

        /*if ((data.enemyName == "ShieldEnemy1" || data.enemyName == "ShieldEnemy2") && dot > 0.5f)
        {
            Debug.Log("Blocked by shield!");
            // TO DO: Play a 'clink' sound or spark effect here
            return; // Exit the function so no damage is taken
        }*/

        // Otherwise, take damage as normal
        currentHealth -= amount;

        //UpdateUI();
        if (currentHealth <= 0) Die();

        // Play sound
        if(playerRef.GetMode() == "comedy")
        {
            int randomNumber = UnityEngine.Random.Range(1, 5);
            AudioManager.audioManagerRef.PlaySound("hitComedy"+randomNumber);
        }
        else
        {
            AudioManager.audioManagerRef.PlaySoundWithRandomPitch("hitTragedy");
        }
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

        // Play sound
        int randomNumber = UnityEngine.Random.Range(1, 4);
        AudioManager.audioManagerRef.PlaySoundWithRandomPitch("enemyDeath"+randomNumber);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            TakeDamage(playerRef.getDamageOutput(), other.transform.position);
            /*currentHealth = Math.Max(currentHealth-playerRef.getDamageOutput(),0);
            if (currentHealth <= 0)
            {
                Die();
            }*/
        }
    }
}
