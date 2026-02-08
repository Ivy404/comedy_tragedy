using System;
using System.Collections;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Unity.Mathematics;

[Serializable]
public struct maskData {
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
    public string additionType; // additive, multiplicative, basepercent

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
        this.additionType="basepercent";
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
    [SerializeField] public GameObject ComedyAsset;
    [SerializeField] public GameObject TragedyAsset;
    [SerializeField] public GameObject textParticle;

    // character stats
    [SerializeField] public maskData comedyMaskData;
    [SerializeField] public maskData tragedyMaskData;
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

    private Rigidbody pRigid;

    //attack control
    private bool attacking;
    private float attackTime;
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

    public UnityEvent onModeSwitch;

    public float lastAttackPerformedTime { get; set; }
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
        //((CapsuleCollider) swordCollider).height = currentData.range;
        //SwordVFX.SetFloat("RotationAngle", currentData.arc);
        lastSwitch = modeSwitchCD;
        lastDmgTaken = iFrames;
        baseComedy = comedyMaskData;
        baseTragedy = tragedyMaskData;

        cameraShake = mainCamera.gameObject.GetComponent<CameraShake>();
        AmbientLight.colorTemperature = tempComedy;

        ComedyAsset.SetActive(true);
        TragedyAsset.SetActive(false);
        pRigid = gameObject.GetComponent<Rigidbody>();
        lastAttackPerformedTime = 0;
        
    }

    // Update is called once per frame
    void Update()
    {
        lastSwitch += Time.deltaTime;
        attackTime += Time.deltaTime;

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
        if (currentData.maskName == "comedy") comedyMaskData.health = currentData.health;
        else if (currentData.maskName == "tragedy") tragedyMaskData.health = currentData.health;

        // debug section
        if (InputSystem.actions.FindAction("DebugUpgrade").WasPerformedThisFrame())
        {
            PlayerAnimator.SetTrigger("attackBow");
        }
    }

    public void addStatUpgrade(statUpgrade upg)
    {
        statUpgrades.Add(upg);

        // HP before applying the upgrades
        float prevHPcomedy = comedyMaskData.maxHealth;
        float prevHPtragedy = tragedyMaskData.maxHealth;
        applyUpgrades();

        // special case for HP upgrades
        if (upg.maxHealth != 0){
            // if it was comedy
            if (upg.maskName == "comedy")
                if (comedyMaskData.maxHealth - prevHPcomedy > 0.0f) {
                    comedyMaskData.health += comedyMaskData.maxHealth - prevHPcomedy;
                } else if (comedyMaskData.maxHealth - prevHPcomedy < 0.0f)
                {
                    comedyMaskData.health = Math.Min(comedyMaskData.health, comedyMaskData.maxHealth);
                }
            // if it was tragedy
            else if (upg.maskName == "tragedy")
                if (tragedyMaskData.maxHealth - prevHPtragedy > 0.0f) {
                    tragedyMaskData.health += tragedyMaskData.maxHealth - prevHPtragedy;
                } else if (tragedyMaskData.maxHealth - prevHPtragedy < 0.0f)
                {
                    tragedyMaskData.health = Math.Min(tragedyMaskData.health, tragedyMaskData.maxHealth);
                }
        }

        // set everything back
        if (currentData.maskName == "comedy") currentData = comedyMaskData;
        else if (currentData.maskName == "tragedy") currentData = tragedyMaskData;
    }

    private float _additiveStat(float baseD, float currentD, float addD)
    {
        return currentD+addD;
    }
    private float _multiplicativeStat(float baseD, float currentD, float multD)
    {
        return currentD*multD;
    }
    private float _basepercentStat(float baseD, float currentD, float dPerc)
    {
        return currentD+baseD*dPerc;
    }

    private void _applySingleUpgrade(ref maskData data, ref maskData baseData, statUpgrade upg)
    {
        Func<float, float, float, float> upgradeAdd = _basepercentStat; // defaults to base percent

        if (upg.additionType == "additive") upgradeAdd = _additiveStat;
        else if (upg.additionType == "multiplicative") upgradeAdd = _multiplicativeStat;
        else if (upg.additionType == "basepercent") upgradeAdd = _basepercentStat;

        data.maxHealth = upgradeAdd(baseData.maxHealth, data.maxHealth, upg.maxHealth);
        data.speed  = upgradeAdd(baseData.speed, data.speed, upg.speed);
        data.attackSpeed = upgradeAdd(baseData.attackSpeed, data.attackSpeed, upg.attackSpeed);
        data.range = upgradeAdd(baseData.range, data.range, upg.range);
        data.arc = upgradeAdd(baseData.arc, data.arc, upg.arc);
        data.damage = upgradeAdd(baseData.damage, data.damage, upg.damage);
    }

    private void _initMaskData(ref maskData data)
    {
        data.maxHealth = 0;
        data.speed = 0;
        data.attackSpeed = 0;
        data.range = 0;
        data.arc = 0;
        data.damage = 0;
    }
    public void applyUpgrades()
    {
        //initialize
        _initMaskData(ref tragedyMaskData);
        _initMaskData(ref comedyMaskData);

        foreach (statUpgrade upg in statUpgrades )
        {
            
            //Debug.Log("applying upgrade..." + upg.maskName);
            if(upg.maskName == "tragedy"){
                _applySingleUpgrade(ref tragedyMaskData, ref baseTragedy, upg);
            } else if (upg.maskName == "comedy")
            {
                _applySingleUpgrade(ref comedyMaskData, ref baseComedy, upg);
            }
        }

    }

    public void takeDamage(float damage)
    {
        if (lastDmgTaken > iFrames){
            currentData.health -= damage;
            lastDmgTaken = 0;

            // Play take damage animation    
            StartCoroutine(LerpOverTime.lerpOverTime(iFrames, t =>
            {
                float easedT = (1f - Mathf.Cos(t * Mathf.PI)) / 2f;
                characterRenderer.material.SetFloat("_Damage_Bool", Mathf.Lerp(1.0f, 0.0f, easedT));
            }));

            // DMG Text
            TextParticle tParticle = Instantiate(textParticle, transform.position+Vector3.up*1.5f, Quaternion.identity).GetComponent<TextParticle>();
            tParticle.SetText(string.Format("-{0:0.}", damage));
            tParticle.SetColor(Color.red);

            if (currentData.health <= 0)
            {
                // TO DO: Inform game manager
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

            // Play sound
            int randomNumber = UnityEngine.Random.Range(1, 7);
            AudioManager.audioManagerRef.PlaySound("playerDamage"+randomNumber);
        }
    }
    
    public void regenHP()
    {
        if (currentData.maskName == "comedy" && tragedyMaskData.health < tragedyMaskData.maxHealth)
            tragedyMaskData.health = Math.Min(tragedyMaskData.health + tragedyMaskData.hpRegen * Time.deltaTime, tragedyMaskData.maxHealth);
        else if (currentData.maskName == "tragedy"  && comedyMaskData.health < comedyMaskData.maxHealth)
            comedyMaskData.health = Math.Min(comedyMaskData.health + comedyMaskData.hpRegen * Time.deltaTime, comedyMaskData.maxHealth);
    }

    public void modeSwitch()
    {
        bool isComedy = currentData.maskName == "comedy";
        VisualEffect transitionVFX = isComedy ? DecrescendoVFX : CrescendoVFX;
        if (!attacking && lastSwitch > modeSwitchCD){
            // data switch
            if (isComedy)
            {
                comedyMaskData = currentData;
                currentData = tragedyMaskData;

                StartCoroutine(LerpOverTime.lerpOverTime(0.5f, t =>
                {
                    AmbientLight.colorTemperature = Mathf.Lerp(tempComedy, tempTragedy, t);
                }));
            } else
            {
                tragedyMaskData = currentData;
                currentData = comedyMaskData;

                StartCoroutine(LerpOverTime.lerpOverTime(0.5f, t =>
                {
                    AmbientLight.colorTemperature = Mathf.Lerp(tempTragedy, tempComedy, t);
                }));
            }
            // Transitions VFX
            AuraVFX.SetBool("IsComedy", !isComedy);
            SwordVFX.SetBool("IsLight", !isComedy);
            transitionVFX.enabled = true;
            transitionVFX.SetFloat("Scale",transitionBuildUp*10f);
            transitionVFX.Play();

            // DMG
            StartCoroutine(crescendoAOE(transitionBuildUp, transform.position));
            PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed); // attackspeed animation
            // SwordVFX.SetFloat("RotationAngle", currentData.arc); //arc?


            // used asset
            ComedyAsset.SetActive(!isComedy);
            TragedyAsset.SetActive(isComedy);
            characterRenderer.material.SetFloat("_IsLight", isComedy ? 0.0f : 1.0f);

            // Reset Variables
            transitionBuildUp = 0.0f;
           
            lastSwitch = 0;
            onModeSwitch.Invoke();
        }
    }

    public string GetMode()
    {
        return currentData.maskName;
    }
    
    public void Move(Vector2 direction)
    {   
        Vector3 movedir = new Vector3(direction.x, 0, direction.y)* currentData.speed;
        Vector3 relativeMove = transform.InverseTransformDirection(movedir).normalized;
        // Set animator to proper vector direction
        PlayerAnimator.SetFloat("MoveX", relativeMove.x, 0.1f, Time.deltaTime);
        PlayerAnimator.SetFloat("MoveZ", relativeMove.z, 0.1f, Time.deltaTime);

        // Move RigidBody
        pRigid.linearVelocity = new Vector3(movedir.x, pRigid.linearVelocity.y, movedir.z );

        if (!attacking)
            if (closestEnemy == null && movedir != Vector3.zero)
                pRigid.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(movedir), rotationSpeed*Time.deltaTime));
            else if (closestEnemy != null) {
                Vector3 enemyDir = closestEnemy.transform.position - transform.position;
                enemyDir.y = 0;
                pRigid.MoveRotation(Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(enemyDir.normalized), rotationSpeed*Time.deltaTime));
            }
    }

    public void Attack()
    {
        if (!attacking && attackTime > currentData.attackRecovery+1.0f/currentData.attackSpeed){
            PlayerAnimator.SetTrigger("attack");
            attackTime = 0;
            attacking = true;
            lastAttackPerformedTime = Time.time;

        }
    }

    IEnumerator crescendoAOE(float scale, Vector3 Iposition)
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
        if (Gamepad.current != null) StartCoroutine(LerpOverTime.lerpOverTime(scale, t =>
        {
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            float speedValues = Mathf.Lerp(scale, 0, t);
            Gamepad.current.SetMotorSpeeds(speedValues, speedValues);
        }));
        // Play sound
        if(GetMode() == "comedy")
            AudioManager.audioManagerRef.PlaySound("crescendoReleaseComedy");
        else
            AudioManager.audioManagerRef.PlaySound("crescendoReleaseTragedy");
    }
    
    public void swordPerformDmg()
    {
        swordCollider.enabled = true;
        // TODO Change VFX
        SwordVFX.enabled = true;
        SwordVFX.Play();
        // Play sound
        if(currentData.maskName == "comedy")
            AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackComedy1");
        else
            AudioManager.audioManagerRef.PlaySoundWithRandomPitch("attackTragedy1");
    }

    public void swordStopDmg()
    {
        attacking = false;
        swordCollider.enabled = false;
        SwordVFX.enabled = false;
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