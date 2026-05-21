using UnityEngine;

public class ItemCheck : MonoBehaviour
{
    [SerializeField] protected GameObject[] requiredItems;

    public bool hasFoundItem;

    public void InventoryCheck()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            Debug.Log(requiredItems[i].name);
            for (int j = 0; j < Inventory.instance.items.Count; j++)
            {
                if (Inventory.instance.items[j] == requiredItems[i].name)
                {
                    Debug.Log(Inventory.instance.items[j]);

                    Debug.Log("Se removió el item " + Inventory.instance.items[j] + " del inventario");
                    Inventory.instance.items.Remove(Inventory.instance.items[j]);
                    
                    hasFoundItem = true;
                    return;
                }
                else
                {
                    Debug.Log(Inventory.instance.items[j]);
                    Debug.Log("No se removió ningún item");
                }
            }
        }
        
    }

    public void ItemFoundReset()
    {
        hasFoundItem = false;
    }
}
