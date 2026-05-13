using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem; // Nuevo Input System

public class SaveStation : MonoBehaviour
{
    public Text messageText;       // referencia al texto UI
    public SaveMenuUI saveMenu;    // referencia al menú de guardado

    void Start()
    {
        messageText.gameObject.SetActive(false); // oculto al inicio
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messageText.gameObject.SetActive(true); // mostrar mensaje
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            messageText.gameObject.SetActive(false); // ocultar mensaje
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && Keyboard.current.eKey.wasPressedThisFrame)
        {
            saveMenu.OpenMenu(); // Abre el menú de guardado
            messageText.gameObject.SetActive(false);
        }
    }
}
