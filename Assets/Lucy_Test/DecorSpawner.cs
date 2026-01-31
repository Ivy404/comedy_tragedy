using UnityEngine;

public class DecorSpawner : MonoBehaviour
{
    public GameObject[] DecorTiles;
    public float spawnChance;

    public bool decorOn;

    private int tileCount;

    private float randomize;

    private void Awake()
    {
        tileCount = DecorTiles.Length;

        spawnDecor();
    }

    private void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "Decor")
        {
            decorOn = true;
        }
        else
        {
            decorOn = false;
        }
    }

    public void spawnDecor()
    {
        if (decorOn == false)
        {
            randomize = Random.Range(0, 100);
            if(randomize < spawnChance)
            {
                GameObject deco = Instantiate(DecorTiles[Random.Range(0, DecorTiles.Length)], new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z), new Quaternion(0, 0, 0, 0)) as GameObject;
            }
            decorOn = false;
        }
    }
}
