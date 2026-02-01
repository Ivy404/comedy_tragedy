using System.Collections;
using UnityEngine;

public class DestroyInSeconds : MonoBehaviour
{
    public int destroyDelay = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(delayedDestroy());
    }

    IEnumerator delayedDestroy()
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(gameObject);
    }
}
