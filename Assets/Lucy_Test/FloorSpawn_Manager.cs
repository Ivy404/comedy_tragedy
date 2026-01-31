using UnityEngine;

public class FloorSpawn_Manager : MonoBehaviour
{
    public GameObject Tile; //Array of tiles 
    public int maxCount = 3;

    public GameObject player;
    private Vector3 playerPos;

    public int tileSize;
    private int tilePos;

    void Start()
    {

        playerPos = player.transform.position;
        tilePos = tileSize * 10;

        // Spawn the 9 tiles

        //mid
        GameObject top = Instantiate(Tile, new Vector3(playerPos.x, 0, playerPos.z + tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject mid = Instantiate(Tile, new Vector3(playerPos.x, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject bot = Instantiate(Tile, new Vector3(playerPos.x, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        //right
        GameObject topR = Instantiate(Tile, new Vector3(playerPos.x + tilePos, 0, playerPos.z + tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject midR = Instantiate(Tile, new Vector3(playerPos.x + tilePos, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject botR = Instantiate(Tile, new Vector3(playerPos.x + tilePos, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        //left
        GameObject topL = Instantiate(Tile, new Vector3(playerPos.x - tilePos, 0, playerPos.z + tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject midL = Instantiate(Tile, new Vector3(playerPos.x - tilePos, 0, playerPos.z), new Quaternion(0, 0, 0, 0)) as GameObject;
        GameObject botL = Instantiate(Tile, new Vector3(playerPos.x - tilePos, 0, playerPos.z - tilePos), new Quaternion(0, 0, 0, 0)) as GameObject;

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
