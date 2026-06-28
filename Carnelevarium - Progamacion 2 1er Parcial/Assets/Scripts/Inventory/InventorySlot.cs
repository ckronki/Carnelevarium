using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [Header("Estado del Slot")]
    public bool ocupado = false; // True si hay un ítem encima

    [Header("Referencia al ítem actual")]
    public InventoryItem itemActual; // El ítem que ocupa este slot (si lo hay)

    private void OnTriggerEnter(Collider other)
    {
        AnchorPoint anchor = other.GetComponent<AnchorPoint>();
        if (anchor != null && !ocupado)
        {
            // Si el AnchorPoint entra y el slot está libre, lo marcamos como potencial punto de anclaje
            anchor.slotDetectado = this;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AnchorPoint anchor = other.GetComponent<AnchorPoint>();
        if (anchor != null && anchor.slotDetectado == this)
        {
            // Si el AnchorPoint sale del área, limpiamos la referencia
            anchor.slotDetectado = null;
        }
    }
}
