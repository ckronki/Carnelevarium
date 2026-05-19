using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class SaveStation : MonoBehaviour
{
    public Text messageText;       // Referencia al texto UI del mundo (Legacy Text)
    public SaveMenuUI saveMenu;    // Referencia al menú de guardado

    void Start()
    {
        messageText.gameObject.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // El mensaje en el mundo siempre te invita a interactuar
            if (messageText != null)
            {
                messageText.text = "Presiona 'E' para acceder a la terminal";
                messageText.color = Color.white;
            }
            messageText.gameObject.SetActive(true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messageText.gameObject.SetActive(false);
        }
    }

    void OnTriggerStay(Collider other)
    {
        // El jugador SIEMPRE puede abrir la terminal con la 'E'
        if (other.CompareTag("Player") && Keyboard.current.eKey.wasPressedThisFrame)
        {
            saveMenu.OpenMenu();
            messageText.gameObject.SetActive(false);
        }
    }
}