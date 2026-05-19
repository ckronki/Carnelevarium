using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SaveStation : MonoBehaviour
{
    public Text messageText;       // Referencia al texto UI del mundo (Legacy Text)
    public SaveMenuUI saveMenu;    // Referencia al menú de guardado

    void Start()
    {
        if (messageText != null)
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (messageText != null)
            {
                // El mensaje en el mundo siempre te invita a interactuar
                messageText.text = "Presiona 'E' para acceder a la terminal";
                messageText.color = Color.white;
                messageText.gameObject.SetActive(true);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (messageText != null)
            {
                messageText.gameObject.SetActive(false);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        // El jugador SIEMPRE puede abrir la terminal con la 'E' si el menú no está ya abierto
        if (other.CompareTag("Player") && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (saveMenu != null && !saveMenu.menuPanel.activeSelf)
            {
                // Ocultamos el texto flotante para que no se quede estorbando detrás del menú
                if (messageText != null) messageText.gameObject.SetActive(false);

                // Abrimos el menú de guardado (esto ya frena el tiempo y libera el mouse)
                saveMenu.OpenMenu();
            }
        }
    }
}