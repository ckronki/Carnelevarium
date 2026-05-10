using UnityEngine;
using UnityEngine.InputSystem;

public class PickupItem : MonoBehaviour
{
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

                inspectSystem.SetItem(itemPrefab, this.gameObject);
            }
        }
    }
}