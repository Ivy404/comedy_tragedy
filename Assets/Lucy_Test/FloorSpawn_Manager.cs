using UnityEngine;

public class FloorSpawn_Manager : MonoBehaviour
{
    public GameObject[] TileTypes; //Array of tiles 
    public int maxCount = 3;

    public GameObject player;
    private Vector3 playerPos;

    private int tileCount;

    public int tileSize;
    private int tilePos;

    void Start()
    {
        tileCount = TileTypes.Length;
        playerPos = player.transform.position;
        tilePos = tileSize * 10;

        // Spawn the 9 tiles

        //mid
        GameObject top = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3 (playerPos.x, 0, playerPos.z + tilePos), new Quaternion (0,0,0,0)) as GameObject;
        GameObject mid = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject bot = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        //right
        GameObject topR = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3 (playerPos.x + tilePos, 0, playerPos.z + tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject midR = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x + tilePos, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject botR = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x + tilePos, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        //left
        GameObject topL = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - tilePos, 0, playerPos.z + tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject midL = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - tilePos, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject botL = Instantiate(TileTypes[Random.Range(0, TileTypes.Length)], new Vector3(playerPos.x - tilePos, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;

        //Set Scale
        top.transform.localScale = new Vector3(tileSize, 1, tileSize);
        mid.transform.localScale = new Vector3(tileSize, 1, tileSize);
        bot.transform.localScale = new Vector3(tileSize, 1, tileSize);

        topR.transform.localScale = new Vector3(tileSize, 1, tileSize);
        midR.transform.localScale = new Vector3(tileSize, 1, tileSize);
        botR.transform.localScale = new Vector3(tileSize, 1, tileSize);

        topL.transform.localScale = new Vector3(tileSize, 1, tileSize);
        midL.transform.localScale = new Vector3(tileSize, 1, tileSize);
        botL.transform.localScale = new Vector3(tileSize, 1, tileSize);
    }

    void Update()
    {
        playerPos = player.transform.position;
    }

    public void SpawnRandom(Vector3 spawnPos, GameObject tile)
    {
        //Spawn with random decor
    }
}
