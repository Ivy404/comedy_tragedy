using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public PlayerActions playerRef;
    public float speed = 1;
    public float maxSpeed = 5;
    public float accelerationRange = 5f;
    public float pickupRange = 5;
    public float floatFactor = 1;
    public float delaySecs = 1;
    private float experience;
    private float spawnTime;
    [SerializeField] private Rigidbody rBody;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeOrb();
    }

    private void InitializeOrb()
    {
        // set random scale for the orb
        float randomNumber = Random.Range(0.3f,1.0f);
        transform.localScale = transform.localScale*randomNumber;
        spawnTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    public void setXP(float xp)
    {
        experience = xp;
    }

    public float getXP()
    {
        return experience;
    }

    private void FollowPlayer()
    {
        Vector3 target = playerRef.gameObject.transform.position+Vector3.up;
        float distance = Vector3.Distance(transform.position, target);
        if(Time.time - spawnTime > delaySecs){
            if (!rBody.isKinematic) rBody.isKinematic = true;

            float t = Mathf.Clamp01(1f - distance / accelerationRange); // time normalization
            float s = Mathf.Lerp(speed, maxSpeed, t);

            transform.position = Vector3.MoveTowards(
                transform.position,
                target,
                s * Time.deltaTime
            );
        } else
        {
            //upspeed -= 0.981f*Time.deltaTime; 
            //transform.position = transform.position+randomDirection*0.02f+Vector3.up*upspeed;

            //transform.position = new Vector3(transform.position.x, transform.position.y+floatFactor*Mathf.Cos(Time.time), transform.position.z);
        }
    }
}
