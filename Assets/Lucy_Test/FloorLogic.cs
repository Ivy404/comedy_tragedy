using UnityEngine;

public class FloorLogic : MonoBehaviour
{
    public GameObject player;
    public GameObject manager;
    public Vector3 playerPos;

    public float distanceX;
    public float distanceZ;

    public float maxDis;
    public float teleDis;

    private void Awake()
    {
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
        }

        if (manager == null)
        {
            manager = GameObject.FindWithTag("GameController");
        }

        Debug.Log(gameObject.transform.localScale.x);

        maxDis = ((manager.GetComponent<FloorSpawn_Manager>().tileSize * 10) + (manager.GetComponent<FloorSpawn_Manager>().tileSize * 10) / 2) ;
        teleDis = (manager.GetComponent<FloorSpawn_Manager>().tileSize * 10) * 3;
    }


    void Update()
    {
        playerPos = player.transform.position;

        distanceX = Mathf.RoundToInt(gameObject.transform.position.x - playerPos.x);
        distanceZ = Mathf.RoundToInt(gameObject.transform.position.z - playerPos.z);

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
