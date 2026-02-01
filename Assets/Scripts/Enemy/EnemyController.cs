using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using System.Collections;
using System;
using UnityEngine.VFX;
using Unity.Mathematics;

public class EnemyController : MonoBehaviour
{
    public EnemyData data;
    public EnemySpawner enemySpawner;
    public PlayerActions playerRef;
    public GameObject hitVFX;
    public Renderer enemyRenderer;

    [Header("Movement Settings")]
    public float separationDistance = 0.5f;
    public float separationForce = 1f;
    public float playerEscapeDistance = 30f;

    [Header("Visuals")]
    public GameObject deathVFXPrefab;
    public Animator enemyAnimator;
	private Transform enemyTransform;
    private NavMeshAgent agent;
    private float lastAttackTime;
    private float currentHealth;
    private bool attacking = false;

    public bool Attacking
    {
        get { return attacking; }
        set { attacking = value; }
    }

    void Start()
    {
        // cache transform
        enemyTransform = GetComponent<Transform>();

        agent = GetComponent<NavMeshAgent>();

        InitializeEnemy();
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
        if(playerRef != null && !attacking)
        {
            MoveAndAvoid();
        }
        // DEBUG
        //if (debugAction.WasPressedThisFrame()) Die();
    }

    void MoveAndAvoid()
    {
        // 1. Calculate Chase Vector
        Vector3 directionToPlayer = (playerRef.transform.position - transform.position).normalized;
        
        // 4. Rotate to face movement
        if (directionToPlayer != Vector3.zero)
        {
            transform.forward = Vector3.Slerp(transform.forward, directionToPlayer, Math.Max(data.rotationSpeed,1f) * Time.deltaTime);
        }

        // Ensure they stay on the floor (keep Y consistent)
        directionToPlayer.y = 0;
        // Update movement if far or try attacking
        float distance = Vector3.Distance(transform.position, playerRef.transform.position);

        if (distance > playerEscapeDistance)
        {
            enemySpawner.Respawn(this);
            Debug.Log("Enemy "+data.name +" respawned");

            if(enemyAnimator != null)
                enemyAnimator.SetBool("walking",true);
            else 
                Debug.LogError("Enemy animator not setup for an enemy!");
        }
        else if (distance > agent.stoppingDistance)
        {
            transform.position += directionToPlayer * Math.Min(data.speed * Time.deltaTime, distance);

            if(enemyAnimator != null)
                enemyAnimator.SetBool("walking",true);
            else 
                Debug.LogError("Enemy animator not setup for an enemy!");
        }
        else
        {
            if(enemyAnimator != null)
                enemyAnimator.SetBool("walking",false);
            else 
                Debug.LogError("Enemy animator not setup for an enemy!");

            TryAttack();
        }

    }

    void TryAttack()
    {
        // Simple attack cooldown using data.attackRate (add this to your ScriptableObject!)
        if (Time.time >= lastAttackTime + 1.5f) 
        {
            Debug.Log($"Attacking player for {data.damage} damage!");

            if(enemyAnimator != null)
                enemyAnimator.SetTrigger("attack");
            else 
                Debug.LogError("Enemy animator not setup for an enemy!");

            //attacking = true;
            //playerRef.takeDamage(data.damage);
            
            lastAttackTime = Time.time;
        }
    }

    public void TryDamagePlayer()
    {
       // Update movement if far or try attacking
        float distance = Vector3.Distance(transform.position, playerRef.transform.position);

        if (distance < agent.stoppingDistance)
            playerRef.takeDamage(data.damage);

        //attacking = false;
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

        Instantiate(hitVFX, transform.position+Vector3.up, Quaternion.identity, transform);
        // play take damage animation    
        StartCoroutine(LerpOverTime(1.0f, t =>
        {
            float easedT = (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
            enemyRenderer.material.SetFloat("_Damage_Bool", Mathf.Lerp(1.0f, 0.0f, easedT));
        }));
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

    IEnumerator LerpOverTime(float duration, Action<float> onUpdate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            onUpdate(t);
            yield return null;
        }

        onUpdate(1f);
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

        if(deathVFXPrefab != null)
        {
            GameObject deathVFX = Instantiate(deathVFXPrefab, transform.position, transform.rotation);
            
            
            VisualEffect effect = deathVFX.GetComponent<VisualEffect>();
            
            if(effect != null)
            {
                Mesh bakedMesh = new Mesh();
                // Dangerous casting
                ((SkinnedMeshRenderer)enemyRenderer).BakeMesh(bakedMesh);
                effect.SetMesh("EnemyMesh", bakedMesh);
                effect.SetFloat("EnemyAngleY",transform.rotation.y);
            }
        } else
        {
            Debug.LogError("Enemy "+data.enemyName+" is missing the death VFX prefab!");
        }

        // TO DO: disable the enemy
        Destroy(gameObject);

        // Play sound
        int randomNumber = UnityEngine.Random.Range(1, 4);
        AudioManager.audioManagerRef.PlaySoundWithRandomPitch("enemyDeath"+randomNumber);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Sword")
        {
            TakeDamage(playerRef.getDamageOutput(), other.transform.position);
        }
    }
}
