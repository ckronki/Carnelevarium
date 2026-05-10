using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Quit()
    {
        Debug.Log("Has cerrado el juego");
        Application.Quit();
    }

}
