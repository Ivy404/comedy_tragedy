using System;
using System.Collections;
using UnityEngine;

public class LerpOverTime : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    public static IEnumerator lerpOverTime(float duration, Action<float> onUpdate)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            onUpdate(t);
            yield return null;
        }

        onUpdate(1f);
    }
}
