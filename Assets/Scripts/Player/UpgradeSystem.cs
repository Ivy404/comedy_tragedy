using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeSystem : MonoBehaviour
{
    [SerializeField] public float commonWeight=5;
    [SerializeField] public float uncommonWeight=3;
    [SerializeField] public float rareWeight=1;
    [SerializeField] public PlayerActions playerActions;
    [SerializeField] public List<statUpgrade> statUpgrades;

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
        float prob = Random.Range(0.0f,1.0f);
        Debug.Log(prob);
        int indexToUse = 0;
        for(int j=0; j < weights.Length;j++)
        {
            if (prob < weights[j]) break;
            indexToUse = j;
        }
        Debug.Log(statUpgrades[indexToUse].upgradeName);
        return statUpgrades[indexToUse];
    }

}