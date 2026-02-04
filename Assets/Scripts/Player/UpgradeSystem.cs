using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] public float commonWeight=5;
    [SerializeField] public float uncommonWeight=3;
    [SerializeField] public float rareWeight=1;
    [SerializeField] public GameManager gameManager;
    [SerializeField] public PlayerActions playerActions;
    [SerializeField] public List<statUpgrade> statUpgrades;

    private float lastLevelUpXp;
    private float totalXP;
    private float level;
    private double nextLevelXP;

    private float[] weights;
    void Awake()
    {
        weights = new float[statUpgrades.Count];
        float cummulativeSum = 0, cWeight = 0;
        int i = 0;
        foreach (statUpgrade upg in statUpgrades)
        {
            if (upg.rarity == "common") cWeight = commonWeight;
            if (upg.rarity == "uncommon") cWeight = commonWeight;
            if (upg.rarity == "rare") cWeight = commonWeight;
            cummulativeSum += cWeight;
            weights[i] = cummulativeSum;
            i++;
        }
        
        for(int j=0; j < statUpgrades.Count;j++)
        {
            weights[j] = weights[j]/cummulativeSum;
        }

        level = 0;
        nextLevelXP = xpAtLevel(level);
        lastLevelUpXp = 0;
        
        gameManager.setLevel((int)level+1);
    }

    void Update()
    {
        
        if (InputSystem.actions.FindAction("DebugUpgrade").WasPressedThisFrame())
        {
            statUpgrade randomUpg = getRandomStatUpgrade();
            playerActions.addStatUpgrade(randomUpg);
        }
    }

    public statUpgrade getRandomStatUpgrade()
    {
        float prob = UnityEngine.Random.Range(0.0f,1.0f);
        //Debug.Log(prob);
        int indexToUse = 0;
        for(int j=0; j < weights.Length;j++)
        {
            if (prob < weights[j]) break;
            indexToUse = j;
        }
        //Debug.Log(statUpgrades[indexToUse].upgradeName);
        return statUpgrades[indexToUse];
    }

    private double xpAtLevel(float level)
    {
        if (level == -1) return 0;
        return 100*Math.Pow(1.5, level);
    }
    private void XPGained(float xp)
    {
        totalXP += xp;
        if (totalXP > nextLevelXP)
        {
            lastLevelUpXp = totalXP;
            level+=1;
            nextLevelXP = xpAtLevel(level);
            gameManager.setLevel((int)level+1);
            gameManager.upgradeUION();
        }
        gameManager.setXPProgress((float)((totalXP-lastLevelUpXp)/(nextLevelXP-lastLevelUpXp)));
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Experience"){
            ExperienceOrb xpOrb = other.GetComponent<ExperienceOrb>();
            XPGained(xpOrb.getXP());
            Destroy(other.gameObject);
        }

    }

}