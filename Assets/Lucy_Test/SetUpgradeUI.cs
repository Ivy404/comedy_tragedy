using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SetUpgradeUI : MonoBehaviour
{
    public GameObject Name;
    public GameObject mask;
    public Sprite[] maskTypes;
    public GameObject rare;

    public void setUI(string UpgradeName, string maskName, string rarity)
    {
        Name.GetComponent<TextMeshProUGUI>().text = UpgradeName;

        //Debug.Log("UPGRADENAME!!" + UpgradeName);
        if (maskName == "comedy")
        {
            mask.GetComponent<Image>().sprite = maskTypes[0];
        }
        else
        {
            mask.GetComponent<Image>().sprite = maskTypes[1];
        }

        if (rarity == "common")
        {
            rare.GetComponent<Image>().color = new Color32(255, 255, 225, 255);
        }
        else if (rarity == "uncommon")
        {
            rare.GetComponent<Image>().color = new Color32(255, 190, 0, 255);
        }
        else if (rarity == "rare")
        {
            rare.GetComponent<Image>().color = new Color32(255, 81, 0, 255);
        }
    }
}

