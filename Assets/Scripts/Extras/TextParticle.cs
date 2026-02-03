using TMPro;
using UnityEngine;

public class TextParticle : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float floatDistance = 2f;
    public float floatDuration = 2f;
    private float lastUpdate;
    private Vector3 initPosition;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastUpdate=0;
        initPosition = transform.position;
    }
    void LateUpdate()
    {
        lastUpdate += Time.deltaTime;
        if (lastUpdate > floatDuration) Destroy(this.gameObject);
        float easedT = 1f - Mathf.Pow(1f - lastUpdate/floatDuration, 2f);
        transform.localPosition = Vector3.Lerp(initPosition,initPosition + Vector3.up*floatDistance, easedT);

        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
        Camera.main.transform.rotation * Vector3.up);
        //transform.rotation = Quaternion.Euler(new Vector3(0,180,0));
        
    }

    public void SetText(string txt)
    {
        text.text = txt;
    }
}
