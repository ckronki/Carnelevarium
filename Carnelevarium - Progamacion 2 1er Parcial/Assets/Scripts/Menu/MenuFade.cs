using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MenuFade : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup titleGroup;
    public CanvasGroup menuGroup;

    [Header("Fade Settings")]
    public float fadeDuration = 1.5f;

    public IEnumerator FadeOutAll()
    {
        float time = 0;
        float startAlphaTitle = titleGroup.alpha;
        float startAlphaMenu = menuGroup.alpha;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            titleGroup.alpha = alpha;
            menuGroup.alpha = alpha;
            yield return null;
        }

        titleGroup.interactable = false;
        titleGroup.blocksRaycasts = false;
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;
    }
}
