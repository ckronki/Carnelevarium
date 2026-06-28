using UnityEngine;

public class FlickeringLights : MonoBehaviour
{
    private Light light;

    public float minIntensity = .5f;
    public float maxIntensity = 5.0f;
    public float flickerSpeed = 0.05f;

    private void Start()
    {
        light = GetComponent<Light>();
        InvokeRepeating("Flicker", 0f, flickerSpeed);

    }
    private void Flicker()

    {
      float randomIntensity = Random.Range(minIntensity, maxIntensity);
        light.intensity = randomIntensity;
    }


     
}
