using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuGameManager : MonoBehaviour
{
    public Camera mainCamera;
    public Transform startPoint, menuPoint, optionsPoint, exitPoint;
    public float transitionDuration = 2f;

    private MenuControls controls;
    private bool started = false;

    // Botones principales
    public List<GameObject> menuButtons;     // Play + Options + Credits + Exit
    public List<GameObject> optionsButtons;  // Back


    // ?? Slider de opciones
    public GameObject optionsSlider;

    public FadeController fadeController;    // referencia al panel negro

    void Awake()
    {
        controls = new MenuControls();
        controls.UI.AnyKey.performed += ctx => OnAnyKey();
    }

    void OnEnable() => controls.UI.Enable();
    void OnDisable() => controls.UI.Disable();

    void Start()
    {
        mainCamera.transform.position = startPoint.position;
        mainCamera.transform.rotation = startPoint.rotation;

        SetButtonsInvisible(menuButtons);
        SetButtonsInvisible(optionsButtons);


        // Desactivar slider al inicio
        if (optionsSlider) optionsSlider.SetActive(false);
    }

    void OnAnyKey()
    {
        if (!started)
        {
            GoToPoint(menuPoint, "Menu");
            started = true;
            FindObjectOfType<StartText>().HideText();
        }
    }

    // Acciones principales
    public void PlayGame()
    {
        int nextIndex = SceneManager.GetActiveScene().buildIndex + 1;
        StartCoroutine(fadeController.FadeOutAndLoad(nextIndex));
    }

    public void ShowOptions()
    {
        GoToPoint(optionsPoint, "Options");
        if (optionsSlider) optionsSlider.SetActive(true);   // activar slider
    }


    public void BackToMenu()
    {
        GoToPoint(menuPoint, "Menu");
        if (optionsSlider) optionsSlider.SetActive(false);  // ocultar slider
    }

    public void ExitGame()
    {
        GoToPoint(exitPoint, "Exit");
        StartCoroutine(fadeController.FadeOutAndQuit());
    }

    // Movimiento de cámara
    public void GoToPoint(Transform target, string pointType)
    {
        StartCoroutine(MoveCamera(target, pointType));
    }

    IEnumerator MoveCamera(Transform target, string pointType)
    {
        Vector3 startPos = mainCamera.transform.position;
        Quaternion startRot = mainCamera.transform.rotation;
        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;
            mainCamera.transform.position = Vector3.Lerp(startPos, target.position, t);
            mainCamera.transform.rotation = Quaternion.Slerp(startRot, target.rotation, t);
            yield return null;
        }

        mainCamera.transform.position = target.position;
        mainCamera.transform.rotation = target.rotation;

        // Activar/desactivar botones según el menú
        if (pointType == "Menu")
        {
            SetButtonsFade(menuButtons, true);
            SetButtonsFade(optionsButtons, false);

        }
        else if (pointType == "Options")
        {
            SetButtonsFade(menuButtons, false);
            SetButtonsFade(optionsButtons, true);
        }
        else if (pointType == "Credits")
        {
            SetButtonsFade(menuButtons, false);
 
        }
        else if (pointType == "Exit")
        {
            SetButtonsFade(menuButtons, false);

        }
    }

    // Helpers
    void SetButtonsFade(List<GameObject> buttons, bool fadeIn)
    {
        foreach (GameObject btn in buttons)
        {
            TextFade tf = btn.GetComponent<TextFade>();
            MenuTextButton mtb = btn.GetComponent<MenuTextButton>();
            if (tf != null)
            {
                if (fadeIn)
                {
                    tf.FadeIn(1f);
                    if (mtb != null) mtb.isActive = true;
                }
                else
                {
                    tf.FadeOut(1f);
                    if (mtb != null) mtb.isActive = false;
                }
            }
        }
    }

    void SetButtonsInvisible(List<GameObject> buttons)
    {
        foreach (GameObject btn in buttons)
        {
            TextFade tf = btn.GetComponent<TextFade>();
            MenuTextButton mtb = btn.GetComponent<MenuTextButton>();
            if (tf != null) tf.SetInvisible();
            if (mtb != null) mtb.isActive = false;
        }
    }
}
