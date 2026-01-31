using UnityEngine;

public class DecorSpawner : MonoBehaviour
{
    public GameObject[] DecorTiles;
    public float spawnChance;

    private bool decorOn;

    private int tileCount;
    private int tileSize;

    public GameObject manager;

    private void Awake()
    {
        tileCount = DecorTiles.Length;

        if (manager == null)
        {
            manager = GameObject.FindWithTag("GameController");
        }

        tileSize = manager.GetComponent<FloorSpawn_Manager>().tileSize;

        spawnDecor();
    }

    private void OnTriggerEnter(Collider other)
    {
        decorOn = false;

        if (other.gameObject.tag == "Decor")
        {
            decorOn = true;
        }
    }

    public void spawnDecor()
    {
        if (decorOn == false)
        {
            GameObject deco = Instantiate(DecorTiles[Random.Range(0, DecorTiles.Length)], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), new Quaternion(0, 0, 0, 0)) as GameObject;
            deco.transform.localScale = new Vector3(tileSize, tileSize, tileSize);
        }
    }
}
