using UnityEngine;
using TMPro;
using System.Collections;

public class TextFade : MonoBehaviour
{
    private TextMeshPro textMesh;

    void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void SetInvisible()
    {
        Color c = textMesh.color;
        c.a = 0f;
        textMesh.color = c;
    }

    public void FadeIn(float duration = 0.1f)
    {
        StartCoroutine(FadeText(0f, 1f, duration));
    }

    public void FadeOut(float duration = 2.5f)
    {
        StartCoroutine(FadeText(1f, 0f, duration));
    }

    IEnumerator FadeText(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        Color c = textMesh.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            textMesh.color = c;
            yield return null;
        }

        c.a = endAlpha;
        textMesh.color = c;
    }
}
