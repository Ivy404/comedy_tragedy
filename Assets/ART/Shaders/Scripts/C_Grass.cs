using UnityEngine;

[ExecuteAlways]
public class GrassTracker : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        if (player != null)
        {
            Shader.SetGlobalVector("_TrackerPosition", player.position);
        }
    }
}