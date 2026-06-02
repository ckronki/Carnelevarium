using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;    
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject cameraObject;

    public GameObject keypad;

    [SerializeField] InspectItem inspectSystem;

    private Keyboard keyboard;

    private void Awake()
    {
        keyboard = Keyboard.current; 
    }
    void Update()
    {
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (keypad.activeInHierarchy)
            {
                Debug.Log("El player no puede pausar");
                return;
            }

            else
            {
                if (InspectItem.Instance.IsInspecting()) return;
                else
                {
                    if (GameIsPaused)
                        Resume();
                    else
                        Pause();
                }
            }
            
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        // Reactivar script de cámara
        if (cameraObject != null)
        {
            var camController = cameraObject.GetComponent<CameraController>();
            if (camController != null)
                camController.enabled = true;
        }

       cameraObject.GetComponent<CameraController>().UnlockCamera();
    }
    private void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; 
        GameIsPaused = true;

        // Desactivar script de cámara
        if (cameraObject != null)
        {
            var camController = cameraObject.GetComponent<CameraController>();
            if (camController != null)
                camController.enabled = false;
        }

        cameraObject.GetComponent<CameraController>().LockCamera();

    }
    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0); 
    }
    public void QuitGame()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }
}
