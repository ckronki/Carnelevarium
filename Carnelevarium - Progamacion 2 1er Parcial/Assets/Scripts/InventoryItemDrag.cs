using UnityEngine;

public class InventoryItemDrag : MonoBehaviour
{
    [HideInInspector] public InventoryItem item;
    [HideInInspector] public Collider miCollider;
    private InventorySystem inventorySystem;

    void Start()
    {
        item = GetComponent<InventoryItem>();
        miCollider = GetComponent<Collider>();
        inventorySystem = InventorySystem.Instance;
    }

    public void RegresarAUltimoSlot()
    {
        if (item != null && item.lastSlot != null && inventorySystem != null)
        {
            inventorySystem.ForzarColocacionFísica(item, item.lastSlot);
        }
    }
}