using UnityEngine;

public class ShakeCamara : MonoBehaviour
{
    public static ShakeCamara Instance;
    private Transform camTransform;
    private Vector3 originalPos;

    private void Awake()
    {
        Instance = this;
        camTransform = Camera.main.transform;
        originalPos = camTransform.localPosition;
    }

    public void Shake(float duration)
    {
        StartCoroutine(ShakeRoutine(duration));
    }

    private System.Collections.IEnumerator ShakeRoutine(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            camTransform.localPosition = originalPos + Random.insideUnitSphere * 0.2f;
            elapsed += Time.deltaTime;
            yield return null;
        }
        camTransform.localPosition = originalPos;
    }
}
