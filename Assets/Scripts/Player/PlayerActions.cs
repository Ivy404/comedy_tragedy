using System;
using UnityEditorInternal;
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
   
}

public class PlayerActions : MonoBehaviour
{
    [SerializeField] public Transform PlayerTransform;
    [SerializeField] public Camera mainCamera;
    [SerializeField] public Animator PlayerAnimator;
    [SerializeField] public VisualEffect AuraVFX;
    [SerializeField] public VisualEffect SwordVFX;

    // character stats
    [SerializeField] private maskData comedyMaskData;
    [SerializeField] private maskData tragedyMaskData;
    [SerializeField] public float crescendoBuildupRate = 0.01f;
    private maskData currentData;

    //attack control
    private bool attacking;
    private float attackTime;
    
    // transition contorl
    private float transitionBuilup;
    [SerializeField] private GameObject CharacterMaterial;

    private Renderer characterRenderer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackTime = 0;
        attacking = false;
        currentData = comedyMaskData;
        transitionBuilup = 0;
        AuraVFX.enabled = true;
        characterRenderer = CharacterMaterial.GetComponent<Renderer>();
        characterRenderer.material.SetFloat("_IsLight", 1.0f);
        SwordVFX.SetBool("IsLight", true);
        PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
    }

    // Update is called once per frame
    void Update()
    {
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

        regenHP();
        if (transitionBuilup < 1.0)
        {
            transitionBuilup = Math.Min(1.0f, transitionBuilup+crescendoBuildupRate*Time.deltaTime);
        }
        AuraVFX.SetFloat("GlowSize", 10*transitionBuilup);
    }


    public void takeDamage(float damage)
    {
        // play take damage animation
        currentData.health -= damage;
        if (currentData.health <= 0)
        {
            Debug.Log("you lose");
        }
    }
    public void regenHP()
    {
        if ( currentData.maskName == "comedy" && tragedyMaskData.health < tragedyMaskData.maxHealth)
        {
            tragedyMaskData.health = Math.Min(tragedyMaskData.health + tragedyMaskData.hpRegen * Time.deltaTime, tragedyMaskData.maxHealth);
        } else if (currentData.maskName == "tragedy"  && comedyMaskData.health < comedyMaskData.maxHealth)
        {
            comedyMaskData.health = Math.Min(comedyMaskData.health + comedyMaskData.hpRegen * Time.deltaTime, comedyMaskData.maxHealth);
        }
    }


    public void modeSwitch()
    {
        if ( currentData.maskName == "comedy")
        {
            AuraVFX.SetBool("IsComedy", false);
            SwordVFX.SetBool("IsLight", false);
            comedyMaskData = currentData;
            currentData = tragedyMaskData;
            transitionBuilup = 0.0f;
            characterRenderer.material.SetFloat("_IsLight", 0.0f);
            PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
        } else
        {
            AuraVFX.SetBool("IsComedy", true);
            SwordVFX.SetBool("IsLight", true);
            tragedyMaskData = currentData;
            currentData = comedyMaskData;
            transitionBuilup = 0.0f;
            characterRenderer.material.SetFloat("_IsLight", 1.0f);
            PlayerAnimator.SetFloat("speedMultiplier", currentData.attackSpeed);
        }
    }
    public void Move(Vector2 direction)
    {   
        if (direction != Vector2.zero)
        {
            PlayerAnimator.SetBool("walk", true);
            Vector3 movedir = new Vector3(direction.x, 0, direction.y);
            PlayerTransform.position += movedir * currentData.speed * Time.deltaTime;
            PlayerTransform.rotation = Quaternion.LookRotation(movedir);
        } else
        {
            PlayerAnimator.SetBool("walk", false);
        }
    }

    public void Attack()
    {
        if (!attacking){
            PlayerAnimator.SetTrigger("attack");
            attacking = true;
        }
    }

    public void performDmg()
    {
        SwordVFX.enabled = true;
        SwordVFX.Play();
    }

}