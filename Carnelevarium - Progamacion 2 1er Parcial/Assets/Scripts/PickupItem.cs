using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.VirtualTexturing;

public class PickupItem : MonoBehaviour
{
    [Header("Identificación única")]
    public string objectID; // Código único (Ej: "Linterna_Pasillo_01")

    [Header("Prefabs y Referencias Visuales")]
    public GameObject itemPrefab; // Prefab base con 'InventoryItem'
    public GameObject flashlightObject; // Modelo 3D visual del suelo

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
            if (inspectSystem != null && !inspectSystem.IsInspecting())
            {
                inspectSystem.SetItem(itemPrefab, this.gameObject, objectID);
            }
        }
    }

    // Invocado automáticamente por el InspectItem al aceptar/guardar el objeto
    public void GuardadoExitosoEnInventario()
    {
        // 1. Enviamos los datos de recolección al SaveSystem de forma inmediata
        SaveSystem ss = FindObjectOfType<SaveSystem>();
        if (ss != null)
        {
            // Añadimos el string lógico al inventario del SaveSystem
            if (ss.inventory != null && itemPrefab != null)
            {
                string nombreLimpio = itemPrefab.name.ToLower().Trim();
                ss.inventory.AddItem(nombreLimpio, false);
            }
            ss.RegistrarObjetoDestruido(this.objectID);
            gameObject.SetActive(false); ;
        }

    }
}