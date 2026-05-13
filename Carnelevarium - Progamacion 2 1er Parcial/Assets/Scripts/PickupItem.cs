using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
    public string objectID; // <--- ASIGNA UN NOMBRE ÚNICO EN EL INSPECTOR (Ej: Pocion_1)
    public GameObject itemPrefab;
    public GameObject flashlightObject;
    private Inventory inventory;
    private bool playerInRange = false;
    private InspectItem inspectSystem;

    void Start()
    {
        inspectSystem = FindObjectOfType<InspectItem>();
        inventory = FindObjectOfType<Inventory>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = true;
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInRange = false;
    }

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!inspectSystem.IsInspecting())
            {
                // --- LÓGICA DE GUARDADO ---
                SaveSystem ss = FindObjectOfType<SaveSystem>();
                if (ss != null)
                {
                    ss.RegistrarObjetoDestruido(objectID);
                }

                if (inventory != null)
                {
                    inventory.AddItem(gameObject.name);
                }

                if (flashlightObject != null)
                {
                    flashlightObject.SetActive(true);
                    Light light = flashlightObject.GetComponent<Light>();
                    if (light != null)
                        light.enabled = false;
                }

                // Cambiamos 'this.gameObject' por 'null' para que InspectItem
                // NO destruya el objeto original al terminar la inspección.
                inspectSystem.SetItem(itemPrefab, null);

                // En lugar de destruir, simplemente ocultamos el objeto de la escena
                this.gameObject.SetActive(false);
            }
        }
    }
}