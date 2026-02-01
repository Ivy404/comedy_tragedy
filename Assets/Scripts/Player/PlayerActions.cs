using System;
using UnityEngine;
using UnityEngine.VFX;

[Serializable]
struct maskData {
    //Variable declaration
    public string maskName;
    public float maxHealth;
    public float health;
    public float hpRegen;
    public float speed;
    public float attackSpeed;
    public float range;
    public float arc;
    public float damage;
   
}

public class PlayerActions : MonoBehaviour
{
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Animator PlayerAnimator;
    [SerializeField] public VisualEffect AuraVFX;
    [SerializeField] public VisualEffect SwordVFX;
    [SerializeField] public VisualEffect CrescendoVFX;

    // character stats
    [SerializeField] private maskData comedyMaskData;
    [SerializeField] private maskData tragedyMaskData;
    [SerializeField] public float crescendoBuildupRate = 0.01f;
    [SerializeField] public Collider swordCollider;
    [SerializeField] public Transform swordPivot;
    [SerializeField] public float lockRadius = 5;
    [SerializeField] public float rotationSpeed = 10;
    [SerializeField] public float modeSwitchCD = 3;
    [SerializeField] public float crescendoDamage = 20;
    private maskData currentData;

    //attack control
    private bool attacking;
    private float attackTime;
    private float colliderTime;

    private bool colliderDmg;
    private float lifetime;
    private GameObject closestEnemy;
    [SerializeField] public GameObject enemiesObject;
    
    
    // transition contorl
    private float transitionBuildUp;
    private float lastSwitch;
    [SerializeField] private GameObject CharacterMaterial;

    private Renderer characterRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTime = 0;
        closestEnemy = null;
        attacking = false;
        currentData = comedyMaskData;
        transitionBuildUp = 0;
        AuraVFX.enabled = true;
        characterRenderer = CharacterMaterial.GetComponent<Renderer>();
        characterRenderer.material.SetFloat("_IsLight", 1.0f);
        SwordVFX.SetBool("IsLight", true);
        PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
        colliderDmg = false;
        ((CapsuleCollider) swordCollider).height = currentData.range;
        lifetime = SwordVFX.GetFloat("Light_Lifetime");
        SwordVFX.SetFloat("RotationAngle", currentData.arc);
        lastSwitch = modeSwitchCD;
        
    }

    // Update is called once per frame
    void Update()
    {
        lastSwitch += Time.deltaTime;
        if (attacking)
        {
            attackTime += Time.deltaTime;
            if (attackTime > 1/currentData.attackSpeed)
            {
                attacking = false;
                SwordVFX.enabled = false;
                attackTime = 0;
            }
        }

        if (colliderDmg)
        {
            colliderTime+=Time.deltaTime;
            Vector3 newAngles = new Vector3(0,Mathf.LerpAngle(-currentData.arc/2, currentData.arc/2, colliderTime/lifetime),0);
            swordPivot.localEulerAngles = newAngles;
            if (colliderTime > lifetime)
            {
                colliderDmg = false;
                swordCollider.enabled = false;
                swordPivot.localEulerAngles = new Vector3(0,-currentData.arc/2,0);
            }
        }


        regenHP();
        if (transitionBuildUp < 1.0)
        {
            transitionBuildUp = Math.Min(1.0f, transitionBuildUp+crescendoBuildupRate*Time.deltaTime);
        }
        AuraVFX.SetFloat("GlowSize", 10*transitionBuildUp);
        closestEnemy = getClosestEnemyDirection();
    }


    public void takeDamage(float damage)
    {
        // play take damage animation
        currentData.health -= damage;
        
        if (currentData.health <= 0)
        {
            // TO DO: Inform game manager
            Debug.Log("you lose");
        }

        // Play sound
        int randomNumber = UnityEngine.Random.Range(1, 7);
        AudioManager.audioManagerRef.PlaySound("playerDamage"+randomNumber);
    }
    public void regenHP()
    {
        if (currentData.maskName == "comedy" && tragedyMaskData.health < tragedyMaskData.maxHealth)
        {
            tragedyMaskData.health = Math.Min(tragedyMaskData.health + tragedyMaskData.hpRegen * Time.deltaTime, tragedyMaskData.maxHealth);
        } else if (currentData.maskName == "tragedy"  && comedyMaskData.health < comedyMaskData.maxHealth)
        {
            comedyMaskData.health = Math.Min(comedyMaskData.health + comedyMaskData.hpRegen * Time.deltaTime, comedyMaskData.maxHealth);
        }
    }


    public void modeSwitch()
    {
        if (!attacking && lastSwitch > modeSwitchCD){
            if ( currentData.maskName == "comedy")
            {
                AuraVFX.SetBool("IsComedy", false);
                SwordVFX.SetBool("IsLight", false);
                comedyMaskData = currentData;
                currentData = tragedyMaskData;
                transitionBuildUp = 0.0f;
                characterRenderer.material.SetFloat("_IsLight", 0.0f);
                PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
                ((CapsuleCollider) swordCollider).height = currentData.range;
                lifetime = SwordVFX.GetFloat("Dark_Lifetime");
                SwordVFX.SetFloat("RotationAngle", currentData.arc);
                //CrescendoVFX.SetFloat("Scale",transitionBuildUp*10);
                //CrescendoVFX.Play();
            } else
            {
                AuraVFX.SetBool("IsComedy", true);
                SwordVFX.SetBool("IsLight", true);
                CrescendoVFX.SetFloat("Scale",transitionBuildUp*10f);
                CrescendoVFX.Play();
                crescendo(transitionBuildUp);

                tragedyMaskData = currentData;
                currentData = comedyMaskData;
                transitionBuildUp = 0.0f;
                characterRenderer.material.SetFloat("_IsLight", 1.0f);
                PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
                ((CapsuleCollider) swordCollider).height = currentData.range;
                CrescendoVFX.enabled = true;
                lifetime = SwordVFX.GetFloat("Light_Lifetime");
                SwordVFX.SetFloat("RotationAngle", currentData.arc);
            }
            lastSwitch = 0;
        }
    }

    public string GetMode()
    {
        return currentData.maskName;
    }
    
    public void Move(Vector2 direction)
    {   
        if (direction != Vector2.zero)
        {
            PlayerAnimator.SetBool("walk", true);
            Vector3 movedir = new Vector3(direction.x, 0, direction.y);
            PlayerTransform.position += movedir * currentData.speed * Time.deltaTime;
            if (closestEnemy == null)
                PlayerTransform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(movedir), rotationSpeed*Time.deltaTime);
            else {
                Vector3 enemyDir = closestEnemy.transform.position - transform.position;
                enemyDir.y = 0;
                PlayerTransform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(enemyDir.normalized), rotationSpeed*Time.deltaTime);
            }
        } else
        {
            if (closestEnemy != null){
                Vector3 enemyDir = closestEnemy.transform.position - transform.position;
                enemyDir.y = 0;
                PlayerTransform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(enemyDir.normalized), rotationSpeed*Time.deltaTime);
            }
            PlayerAnimator.SetBool("walk", false);
        }
    }

    public void Attack()
    {
        if (!attacking){
            PlayerAnimator.SetTrigger("attack");
            attackTime = 0;
            attacking = true;

            // Play sound
            if(currentData.maskName == "comedy")
                AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackComedy1");
            else
                AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackTragedy1");
        }
    }

    public void crescendo(float scale)
    {
        float radius = scale*7+2;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, radius);

        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<EnemyController>().TakeDamage(crescendoDamage*scale, transform.position);
            }
        }
    }

    public void performDmg()
    {
        swordCollider.enabled = true;
        colliderDmg = true;
        colliderTime = 0;
        SwordVFX.enabled = true;
        SwordVFX.Play();
    }

    public float getDamageOutput()
    {
        return currentData.damage;
    }

    private GameObject getClosestEnemyDirection()
    {
        float minDist = lockRadius;
        GameObject currentEnemy = null;
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, lockRadius);

        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
                // Push away from neighbors
                Vector3 diff = transform.position - col.transform.position;
                if (diff.magnitude < minDist)
                {
                    minDist = diff.magnitude;
                    currentEnemy = col.gameObject;
                }
            }
        }
        return currentEnemy;
    }

}