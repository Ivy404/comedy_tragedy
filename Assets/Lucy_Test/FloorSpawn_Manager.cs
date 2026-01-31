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

        // Spawn the 9 tiles
        //mid
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3 (playerPos.x, 0, playerPos.z + 10), new Quaternion (0,0,0,0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x, 0, playerPos.z), new Quaternion(0, 0, 0, 0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x, 0, playerPos.z - 10), new Quaternion(0, 0, 0, 0));
        //right
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3 (playerPos.x + 10, 0, playerPos.z + 10), new Quaternion(0, 0, 0, 0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x + 10, 0, playerPos.z), new Quaternion(0, 0, 0, 0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x + 10, 0, playerPos.z - 10), new Quaternion(0, 0, 0, 0));
        //left
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - 10, 0, playerPos.z + 10), new Quaternion(0, 0, 0, 0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - 10, 0, playerPos.z), new Quaternion(0, 0, 0, 0));
        Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - 10, 0, playerPos.z - 10), new Quaternion(0, 0, 0, 0));
    }

    void Update()
    {
        playerPos = player.transform.position;
    }
}
