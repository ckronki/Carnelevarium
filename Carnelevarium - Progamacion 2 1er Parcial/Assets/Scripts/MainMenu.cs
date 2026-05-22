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
        Application.Quit();

        Debug.Log("Has cerrado el juego");
        
    }

    public void goMenu()
    {
        SceneManager.LoadSceneAsync(0);
    }

}
