using UnityEngine;

public class FloorSpawn_Manager : MonoBehaviour
{
    public GameObject[] TileTypes; //Array of tiles 
    public int maxCount = 3;

    public GameObject player;
    private Vector3 playerPos;
    private int tileCount;

    void Start()
    {
        tileCount = TileTypes.Length;
        playerPos = player.transform.position;

        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], playerPos); // Spawn the 9 tiles
    }

    void Update()
    {
        playerPos = player.transform.position;
    }
}
