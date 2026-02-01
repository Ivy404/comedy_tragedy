using UnityEngine;

public class DecorSpawner : MonoBehaviour
{
    public GameObject[] DecorTiles;
    public float spawnChance;

    public bool decorOn;

    private int tileCount;

    private float randomize;

    public GameObject manager;

    private void Awake()
    {
        tileCount = DecorTiles.Length;

        if (manager == null)
        {
            manager = GameObject.FindWithTag("GameController");
        }

        spawnDecor();
    }

    public void spawnDecor()
    {
        if (Physics.CheckSphere(transform.position, manager.GetComponent<FloorSpawn_Manager>().tileSize * 3f, 2))
        {
            decorOn = true;
        }
        else
        {
            decorOn = false;
            randomize = Random.Range(0, 100);
            if (randomize < spawnChance)
            {
                GameObject deco = Instantiate(DecorTiles[Random.Range(0, DecorTiles.Length)], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), new Quaternion(0, 0, 0, 0)) as GameObject;
            }
        }
    }
}
