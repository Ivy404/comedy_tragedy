using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public Image xpBar;
    public TextMeshProUGUI xpText;
    public float updateRate = 0.2f;
    private float xpP;
    private float lastUpdate;

    void Start()
    {
        xpBar.fillAmount = 0;
        lastUpdate = 0;
        xpText.text = "1";
    }

    void Update()
    {
        lastUpdate+=Time.deltaTime;
        if (Math.Abs(xpBar.fillAmount - xpP) > 10e-6 )
        {
            xpBar.fillAmount = Mathf.Lerp(xpBar.fillAmount, xpP, lastUpdate/updateRate);
        }
    }
    public void setXPProgress(float xpProgress)
    {
        lastUpdate = 0;
        xpP = xpProgress;
    }

    public void setLevel(int level)
    {
        xpText.text = level.ToString();
    }
}
