using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    public string objectID;
    public GameObject itemPrefab; // El objeto con el componente InventoryItem e InventoryItemDrag
    public GameObject flashlightObject;

    private bool playerInRange = false;
    private InspectItem inspectSystem;

    void Start()
    {
        inspectSystem = FindObjectOfType<InspectItem>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!inspectSystem.IsInspecting())
            {
                // Enviar a inspección primero pasándole una referencia de este script 
                // para que sepa qué meter al inventario al terminar de verlo
                inspectSystem.SetItem(itemPrefab, this.gameObject, objectID);
            }
        }
    }

    // Este método lo ejecutará la cámara de inspección cuando el jugador presione ESC o acepte el objeto
    public void GuardadoExitosoEnInventario()
    {
        SaveSystem ss = FindObjectOfType<SaveSystem>();
        if (ss != null) ss.RegistrarObjetoDestruido(objectID);

        // CORRECCIÓN: Si quieres que la linterna desaparezca del suelo/mundo, la desactivamos por completo
        if (flashlightObject != null)
        {
            flashlightObject.SetActive(false);
        }

        // Destruir permanentemente el activador del suelo
        Destroy(this.gameObject);
    }
}