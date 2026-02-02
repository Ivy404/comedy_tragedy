using UnityEngine;

public class ExperienceOrb : MonoBehaviour
{
    public PlayerActions playerRef;
    public float speed = 1;
    private float lastUpdate;
    private Vector3 velocity = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitializeOrb();
    }

    private void InitializeOrb()
    {
        // set random scale for the orb
        float randomNumber = Random.Range(0.8f,1.2f);
        lastUpdate = 0;
        transform.localScale = transform.localScale*randomNumber;
    }

    // Update is called once per frame
    void Update()
    {
        FollowPlayer();
    }

    private void FollowPlayer()
    {
        lastUpdate+=Time.deltaTime;
        transform.position = Vector3.SmoothDamp(transform.position, playerRef.gameObject.transform.position, ref velocity, speed);
    }
}
