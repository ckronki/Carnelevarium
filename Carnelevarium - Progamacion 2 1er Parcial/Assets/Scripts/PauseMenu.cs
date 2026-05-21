using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PauseMenu : MonoBehaviour
{
    public static bool GameIsPaused = false;    
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject cameraObject;

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
            if (inspectSystem.isInspecting) return;
            else
            {
                if (GameIsPaused)
                    Resume();
                else
                    Pause();
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

        // Bloquear cursor de nuevo
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
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

        // Liberar cursor
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
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
