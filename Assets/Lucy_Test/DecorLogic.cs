using UnityEngine;

public class DecorLogic : MonoBehaviour
{
    public int colSize;
    public GameObject spawner;

    private void Start()
    {
        colSize = spawner.GetComponent<FloorSpawn_Manager>().tileSize;
        Collider[] hitColliders;
        hitColliders = Physics.OverlapBox(transform.position, new Vector3(colSize / 2, 1, colSize / 2) , Quaternion.identity, 7 );

        for (int i = hitColliders.Length - 1; i > -1; i--)
        {
            ObjectComponentState(hitColliders[i], true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter");
        ObjectComponentState(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit");
        ObjectComponentState(other, false);
    }

    private void ObjectComponentState(Collider other, bool state)
    {
        other.enabled = state;
    }

    // Draw the Box Overlap as a gizmo to show where it currently is testing. Click the Gizmos button to see this.
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        if (Application.isPlaying)
            // Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
            Gizmos.DrawWireCube(transform.position, transform.localScale);
    }
}
