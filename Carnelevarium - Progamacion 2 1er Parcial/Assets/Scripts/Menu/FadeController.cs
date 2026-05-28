using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class FadeController : MonoBehaviour
{
    public Image fadePanel;

    void Awake()
    {
        fadePanel.gameObject.SetActive(true);
        Color c = fadePanel.color;
        c.a = 0f;
        fadePanel.color = c;
    }

    // Fade y carga escena por nombre
    public IEnumerator FadeOutAndLoad(string sceneName)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneName);
    }

    // ?? Fade y carga escena por índice
    public IEnumerator FadeOutAndLoad(int sceneIndex)
    {
        yield return FadeOut();
        SceneManager.LoadScene(sceneIndex);
    }

    // Fade y salir del juego
    public IEnumerator FadeOutAndQuit()
    {
        yield return FadeOut();
        Application.Quit();
    }

    private IEnumerator FadeOut()
    {
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
    }
}
