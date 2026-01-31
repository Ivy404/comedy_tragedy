using UnityEngine;

public class DecorLogic : MonoBehaviour
{
    private float distance;
    private GameObject player;
    public GameObject manager;

    private bool spawned;

    private float maxDis;


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

        maxDis = manager.GetComponent<FloorSpawn_Manager>().tileSize;
        spawned = true;
    }

    private void Update()
    {
        Debug.Log(maxDis);
        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance > maxDis * 4 && spawned == true)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(false);
            }
            spawned = false;
        }

        if(distance < maxDis * 4 && spawned == false)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(true);
            }
            spawned = true;
        }
    }

    /*
    GameObject spawnedobj;

    private int tileSize;

    public GameObject manager;

    private void Awake()
    {
        //spawn parent
        spawnedobj = new GameObject("Detector");
        //Add Components
        spawnedobj.AddComponent<BoxCollider>();
        //spawnedobj.GetComponent<BoxCollider>().isTrigger = true;
        spawnedobj.tag = "Decor";
        spawnedobj.transform.position = gameObject.transform.position;
        spawnedobj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.parent = spawnedobj.transform;

        if (manager == null)
        {
            manager = GameObject.FindWithTag("GameController");
        }

        tileSize = manager.GetComponent<FloorSpawn_Manager>().tileSize;

        gameObject.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
    }*/
}
