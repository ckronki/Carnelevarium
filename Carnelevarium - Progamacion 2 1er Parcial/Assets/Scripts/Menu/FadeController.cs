using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public Image fadePanel;

    void Awake()
    {
        fadePanel.gameObject.SetActive(false);
    }

    // Recibe el nombre de la escena como string
    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;

        SceneManager.LoadScene(sceneName);
    }

    public IEnumerator FadeOutAndQuit()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        float elapsed = 0f;
        float duration = 1f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;

        Application.Quit();
    }
}
