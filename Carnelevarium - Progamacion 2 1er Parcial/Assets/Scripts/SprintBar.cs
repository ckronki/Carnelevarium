using UnityEngine;
using UnityEngine.UI;

public class SprintBar : MonoBehaviour
{
    public Player player;          // referencia al Player en la escena
    public float sprint;           // stamina actual
    public float maxSprint;        // stamina máxima

    [Header("Interfaz")]
    public Image sprinthBar;       // la imagen de la barra
    public CanvasGroup canvasGroup; // para controlar visibilidad (opcional)

    private void Start()
    {
        maxSprint = player.staminaMax;
        HideBar(); // al inicio oculta la barra
    }

    private void Update()
    {
        if (player != null)
        {
            sprint = player.staminaCurrent;
        }

        updateInterface();
    }

    public void updateInterface()
    {
        sprinthBar.fillAmount = sprint / maxSprint;

        // Mostrar la barra solo si la stamina < max
        if (sprint < maxSprint)
        {
            ShowBar();
        }
        else
        {
            HideBar();
        }
    }

    private void ShowBar()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;   // visible
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
        else
        {
            sprinthBar.gameObject.SetActive(true);
        }
    }

    private void HideBar()
    {
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;   // invisible
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
        }
        else
        {
            sprinthBar.gameObject.SetActive(false);
        }
    }
}
