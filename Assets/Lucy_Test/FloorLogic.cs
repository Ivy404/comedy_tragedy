using UnityEngine;

public class FloorLogic : MonoBehaviour
{
    public GameObject player;
    public Vector3 playerPos;
    private float distanceX;
    private float distanceZ;

    public float maxDis;
    public float teleDis;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        maxDis = (gameObject.transform.localScale.x * 10) * 2;
        teleDis = (gameObject.transform.localScale.x * 10) * 3;
    }


    void Update()
    {
        playerPos = player.transform.position;

        distanceX = Mathf.RoundToInt(gameObject.transform.position.x - playerPos.x);
        distanceZ = Mathf.RoundToInt(gameObject.transform.position.z - playerPos.z);

        Debug.Log(distanceX);

        if (distanceX >= maxDis)
        {
            gameObject.transform.position = gameObject.transform.position - new Vector3(teleDis, 0, 0);
        }

        if (distanceX <= -maxDis)
        {
            gameObject.transform.position = gameObject.transform.position + new Vector3(teleDis, 0, 0);
        }

        if (distanceZ >= maxDis)
        {
            gameObject.transform.position = gameObject.transform.position - new Vector3(0, 0, teleDis);
        }

        if (distanceZ <= -maxDis)
        {
            gameObject.transform.position = gameObject.transform.position + new Vector3(0, 0, teleDis);
        }
    }
}
