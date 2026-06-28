using UnityEngine;

public class AnchorPoint : MonoBehaviour
{
    [Header("Referencia al Slot detectado")]
    public InventorySlot slotDetectado; // Slot con el que colisiona este punto

    [Header("Referencia al ítem padre")]
    public InventoryItem itemPadre;

    void Start()
    {
        if (itemPadre == null)
            itemPadre = GetComponentInParent<InventoryItem>();
    }
}
