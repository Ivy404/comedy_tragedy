using UnityEngine;

public class TeleportToObject : MonoBehaviour
{
    public GameObject referenceObject;

    // Update is called once per frame
    void Update()
    {
        if(referenceObject != null)
            gameObject.transform.position = new Vector3(referenceObject.transform.position.x, referenceObject.transform.position.y, referenceObject.transform.position.z);
    }
}
