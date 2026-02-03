using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{

    public Image xpBar;
    public TextMeshProUGUI xpText;
    public Animator maskAnim;
    public Image comedyBar;
    public Image tragedyBar;
    public TextMeshProUGUI comedyHP;
    public TextMeshProUGUI tragedyHP;
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

    public void setHPComedy(float hp, float maxHp)
    {
        comedyBar.fillAmount = hp/maxHp;
        comedyHP.text = string.Format("{0:0}/{1:0}",hp,maxHp);
    }
    public void setHPTragedy(float hp, float maxHp)
    {
        tragedyBar.fillAmount = hp/maxHp;
        tragedyHP.text = string.Format("{0:0}/{1:0}",hp,maxHp);
    }

    public void onModeSwitch()
    {
        maskAnim.SetBool("isComedy", !maskAnim.GetBool("isComedy"));
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
