using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;

[Serializable]
struct maskData {
    //Variable declaration
    public string maskName;
    public float maxHealth;
    public float health;
    public float hpRegen;
    public float speed;
    public float attackSpeed;
    public float attackRecovery;
    public float range;
    public float arc;
    public float damage;
   
}

[Serializable]
public struct statUpgrade {
    //Variable declaration
    public string upgradeName;
    public string maskName;
    public float maxHealth;
    public float speed;
    public float attackSpeed;
    public float range;
    public float arc;
    public float damage;
    public string rarity;
    public string special;

    public statUpgrade(string upgradeName, string mask)
    {
        this.upgradeName=upgradeName;
        this.maskName=mask;
        this.maxHealth=0;
        this.speed=0;
        this.attackSpeed=0;
        this.range=0;
        this.arc=0;
        this.damage=0;
        this.rarity="";
        this.special="";
    }
   
}

public class PlayerActions : MonoBehaviour
{
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Animator PlayerAnimator;
    [SerializeField] public VisualEffect AuraVFX;
    [SerializeField] public VisualEffect SwordVFX;
    [SerializeField] public VisualEffect CrescendoVFX;
    [SerializeField] public VisualEffect DecrescendoVFX;

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
    [SerializeField] public float iFrames=0.5f;
    [SerializeField] private maskData currentData;
    [SerializeField] private Light AmbientLight;
    [SerializeField] private float tempComedy;
    [SerializeField] private float tempTragedy;

    //attack control
    private bool attacking;
    private float attackTime;
    private float colliderTime;

    private bool colliderDmg;
    private float lifetime;
    private GameObject closestEnemy;
    [SerializeField] public GameObject enemiesObject;
    
    
    // transition control
    private float transitionBuildUp;
    private float lastSwitch;
    private bool crescendoMaxed = false;
    [SerializeField] private GameObject CharacterMaterial;
    private float lastDmgTaken;
    private maskData baseComedy;
    private maskData baseTragedy;
    private Renderer characterRenderer;
    private CameraShake cameraShake;

    // upgradeSystem
    [SerializeField] List<statUpgrade> statUpgrades;
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
        lastDmgTaken = iFrames;
        baseComedy = comedyMaskData;
        baseTragedy = tragedyMaskData;

        cameraShake = mainCamera.gameObject.GetComponent<CameraShake>();
        AmbientLight.colorTemperature = tempComedy;
        
    }

    // Update is called once per frame
    void Update()
    {
        lastSwitch += Time.deltaTime;
        attackTime += Time.deltaTime;
        if (attacking)
        {
            if (attackTime > 1/currentData.attackSpeed)
            {
                attacking = false;
                SwordVFX.enabled = false;
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
        else
        {
            if(!crescendoMaxed)
            {
                crescendoMaxed = true;
                // Play sound
                AudioManager.audioManagerRef.PlaySound("crescendoMaxed");
            }
        }
        AuraVFX.SetFloat("GlowSize", 10*transitionBuildUp);
        closestEnemy = getClosestEnemyDirection();
        lastDmgTaken += Time.deltaTime;
    }

    public void debugDisplayCurrentData()
    {
        Debug.Log("maxhealth " + currentData.maxHealth.ToString());
        Debug.Log("speed " + currentData.speed.ToString());
        Debug.Log("attackSpeed " + currentData.attackSpeed.ToString());
        Debug.Log("range " + currentData.range.ToString());
        Debug.Log("arc " + currentData.arc.ToString());
        Debug.Log("damage " + currentData.damage.ToString());
    }

    public void addStatUpgrade(statUpgrade upg)
    {
        statUpgrades.Add(upg);
        applyUpgrades();
    }
    public void applyUpgrades()
    {
        //initialize
        tragedyMaskData.maxHealth = 0;
        tragedyMaskData.speed = 0;
        tragedyMaskData.attackSpeed = 0;
        tragedyMaskData.range = 0;
        tragedyMaskData.arc = 0;
        tragedyMaskData.damage = 0;
        //initialize
        comedyMaskData.maxHealth = 0;
        comedyMaskData.speed = 0;
        comedyMaskData.attackSpeed = 0;
        comedyMaskData.range = 0;
        comedyMaskData.arc = 0;
        comedyMaskData.damage = 0;
        foreach (statUpgrade upg in statUpgrades )
        {
            
            Debug.Log("applying upgrade..." + upg.maskName);
            if(upg.maskName == "tragedy"){
                tragedyMaskData.maxHealth += baseTragedy.maxHealth*upg.maxHealth;
                tragedyMaskData.speed += baseTragedy.speed*upg.speed;
                tragedyMaskData.attackSpeed += baseTragedy.attackSpeed*upg.attackSpeed;
                tragedyMaskData.range += baseTragedy.range*upg.range;
                tragedyMaskData.arc += baseTragedy.arc*upg.arc;
                tragedyMaskData.damage += baseTragedy.damage*upg.damage;
            } else if (upg.maskName == "comedy")
            {
                comedyMaskData.maxHealth += baseComedy.maxHealth*upg.maxHealth;
                comedyMaskData.speed += baseComedy.speed*upg.speed;
                comedyMaskData.attackSpeed += baseComedy.attackSpeed*upg.attackSpeed;
                comedyMaskData.range += baseComedy.range*upg.range;
                comedyMaskData.arc += baseComedy.arc*upg.arc;
                comedyMaskData.damage += baseComedy.damage*upg.damage;
                
            }
        }
        if (currentData.maskName == "comedy") currentData = comedyMaskData;
        else if (currentData.maskName == "tragedy") currentData = tragedyMaskData;
    }

    public void takeDamage(float damage)
    {
        if (lastDmgTaken > iFrames){
            // play take damage animation    
            StartCoroutine(LerpOverTime(iFrames, t =>
            {
                float easedT = (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
                characterRenderer.material.SetFloat("_Damage_Bool", Mathf.Lerp(1.0f, 0.0f, easedT));
            }));
            currentData.health -= damage;

            if (currentData.health <= 0)
            {
                // TO DO: Inform game manager
                //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            // Play sound
            int randomNumber = UnityEngine.Random.Range(1, 7);
            AudioManager.audioManagerRef.PlaySound("playerDamage"+randomNumber);
        }
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
                DecrescendoVFX.enabled = true;
                SwordVFX.SetFloat("RotationAngle", currentData.arc);
                DecrescendoVFX.SetFloat("Scale",transitionBuildUp*10);
                DecrescendoVFX.Play();
                StartCoroutine(LerpOverTime(0.5f, t =>
                {
                    AmbientLight.colorTemperature = Mathf.Lerp(tempComedy, tempTragedy, t);
                }));
            } else
            {
                AuraVFX.SetBool("IsComedy", true);
                SwordVFX.SetBool("IsLight", true);
                CrescendoVFX.enabled = true;
                CrescendoVFX.SetFloat("Scale",transitionBuildUp*10f);
                CrescendoVFX.Play();
                StartCoroutine(crescendo(transitionBuildUp, transform.position));

                tragedyMaskData = currentData;
                currentData = comedyMaskData;
                transitionBuildUp = 0.0f;
                characterRenderer.material.SetFloat("_IsLight", 1.0f);
                PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
                ((CapsuleCollider) swordCollider).height = currentData.range;
                lifetime = SwordVFX.GetFloat("Light_Lifetime");
                SwordVFX.SetFloat("RotationAngle", currentData.arc);
                StartCoroutine(LerpOverTime(0.5f, t =>
                {
                    AmbientLight.colorTemperature = Mathf.Lerp(tempTragedy, tempComedy, t);
                }));
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
        if (!attacking && attackTime > currentData.attackRecovery+1.0f/currentData.attackSpeed){
            PlayerAnimator.SetTrigger("attack");
            attackTime = 0;
            attacking = true;

        }
    }

    IEnumerator crescendo(float scale, Vector3 Iposition)
    {
        yield return new WaitForSeconds(0.5f);
        cameraShake.AddTrauma(1,1/scale);
        float radius = scale*7+2;
        Collider[] nearbyEnemies = Physics.OverlapSphere(Iposition, radius);

        foreach (var col in nearbyEnemies)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
                col.gameObject.GetComponent<EnemyController>().TakeDamage(crescendoDamage*scale, Iposition);
            }
        }

        crescendoMaxed = false;

        // Play sound
        if(GetMode() == "comedy")
            AudioManager.audioManagerRef.PlaySound("crescendoReleaseComedy");
        else
            AudioManager.audioManagerRef.PlaySound("crescendoReleaseTragedy");
    }

    public void performDmg()
    {
        swordCollider.enabled = true;
        colliderDmg = true;
        colliderTime = 0;
        SwordVFX.enabled = true;
        SwordVFX.Play();
        // Play sound
        if(currentData.maskName == "comedy")
            AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackComedy1");
        else
            AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackTragedy1");
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
}