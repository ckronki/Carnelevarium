using UnityEngine;
using System.Collections;

public class SliderFade : MonoBehaviour
{
    public CanvasGroup canvasGroup;

    void Awake()
    {
        if (canvasGroup == null) canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;
        gameObject.SetActive(false);
    }

    public void FadeIn(float duration = 2.5f)
    {
        gameObject.SetActive(true);
        StartCoroutine(FadeTo(1f, duration));
    }

    public void FadeOut(float duration = 0.1f)
    {
        StartCoroutine(FadeTo(0f, duration, true));
    }

    IEnumerator FadeTo(float targetAlpha, float duration, bool deactivateOnEnd = false)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
        if (deactivateOnEnd && targetAlpha == 0f)
            gameObject.SetActive(false);
    }
}
