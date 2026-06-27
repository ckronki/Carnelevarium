using UnityEngine;

public class BlockedDoor : Door
{
    [SerializeField] private string requiredItem;
    // Nombre exacto del ítem en Inventory (ej: "Crowbar")

    [TextArea][SerializeField] private string openDialogue;

    public override void Interact()
    {
        // Si no tenés el ítem requerido ? diálogo de puerta bloqueada
        if (!Inventory.instance.HasItem(requiredItem))
        {
            StartCoroutine(HasInteracted(dialogue, dialogueTime));
            return;
        }

        // Si lo tenés ? abrir puerta y eliminarlo del inventario
        OpenDoor();
        StartCoroutine(HasInteracted(openDialogue, dialogueTime));

        Inventory.instance.RemoveItem(requiredItem);
        Debug.Log($"{requiredItem} eliminado del inventario tras abrir la puerta.");
    }
}
