using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Vector3 maximumTranslationShake = Vector3.one * 0.5f;
    [SerializeField] private float frequency = 25f;
    [SerializeField] private float recoverySpeed = 1.5f;

    private float seed;
    [SerializeField] private float trauma = 0.0f;

    private Vector3 originalLocalPosition;
    private bool isInitialized = false;

    private void Awake()
    {
        seed = Random.value;
    }

    private void OnEnable()
    {
        originalLocalPosition = transform.localPosition;
        isInitialized = true;
    }

    private void Update()
    {
        if (!isInitialized) return;
        if (trauma == 0) return;

        trauma = Mathf.Clamp01(trauma - recoverySpeed * Time.deltaTime);

        Vector3 shake = new Vector3(
            Mathf.PerlinNoise(seed,         Time.time * frequency) * 2 - 1,
            Mathf.PerlinNoise(seed + 1,     Time.time * frequency) * 2 - 1,
            Mathf.PerlinNoise(seed + 2,     Time.time * frequency) * 2 - 1
        );

        transform.localPosition = originalLocalPosition + Vector3.Scale(shake, maximumTranslationShake) * trauma;
    }

    public void AddTrauma(float amount, float rspeed)
    {
        trauma = Mathf.Clamp01(trauma + amount);
        recoverySpeed = rspeed;
    }
}   