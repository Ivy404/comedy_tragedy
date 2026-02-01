using UnityEngine;

public class DecorDetector : MonoBehaviour
{
    public int colSize;
    public GameObject spawner;

    private void Start()
    {
        colSize = (spawner.GetComponent<FloorSpawn_Manager>().tileSize * 10) * 3;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(colSize, 1, colSize);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Decor")
        {
            if (other.transform.GetChild(0) != null)
            {
                other.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Decor")
        {
            if (other.transform.GetChild(0) != null)
            {
                other.transform.GetChild(0).gameObject.SetActive(false);
            }
        }

        /*
        if (other.tag == "Decor")
        {
            //spawn object
            spawnedobj = new GameObject("Detector");
            //Add Components
            spawnedobj.AddComponent<BoxCollider>();
            spawnedobj.GetComponent<BoxCollider>().isTrigger = true;
            spawnedobj.tag = "Decor";
            spawnedobj.transform.position = other.transform.position;
            other.transform.parent = spawnedobj.transform;

            Debug.Log("Exit");
            other.gameObject.SetActive(false);
        }*/
    }
}
