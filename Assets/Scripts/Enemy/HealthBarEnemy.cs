using UnityEngine;
using UnityEngine.UI;

public class HealthBarEnemy : MonoBehaviour
{

    public Image HPForeground;
    private float hp;
    public float updateRate = 0.2f;
    private float lastUpdate = 0;
    // Update is called once per frame
    void Update()
    {
        lastUpdate += Time.deltaTime;
        if (hp < HPForeground.fillAmount)
        {
            HPForeground.fillAmount = Mathf.Lerp(HPForeground.fillAmount, hp, lastUpdate/updateRate);
        }   
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        Camera.main.transform.rotation * Vector3.up);
        //transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        
    }
    public void setHPPercentage(float amount)
    {
        lastUpdate = 0;
        hp = amount;
    }
}
