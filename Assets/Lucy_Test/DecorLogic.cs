using UnityEngine;

public class DecorLogic : MonoBehaviour
{
    GameObject spawnedobj;

    private void Awake()
    {
        //spawn parent
        spawnedobj = new GameObject("Detector");
        //Add Components
        spawnedobj.AddComponent<BoxCollider>();
        spawnedobj.GetComponent<BoxCollider>().isTrigger = true;
        spawnedobj.tag = "Decor";
        spawnedobj.transform.position = gameObject.transform.position;
        spawnedobj.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        gameObject.transform.parent = spawnedobj.transform;
    }
}
