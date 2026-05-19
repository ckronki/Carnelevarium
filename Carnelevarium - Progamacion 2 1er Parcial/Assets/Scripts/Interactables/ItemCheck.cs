using UnityEngine;

public class ItemCheck : MonoBehaviour , IInteractable
{
    [SerializeField] GameObject[] requiredItems;

    public void Interact()
    {
        for (int i = 0; i < requiredItems.Length; i++)
        {
            Debug.Log(requiredItems[i].name);
            for (int j = 0; j < Inventory.instance.items.Count; j++)
            {
                if (Inventory.instance.items[j] == requiredItems[i].name)
                {
                    Debug.Log(Inventory.instance.items[j]);
                    Inventory.instance.items.Remove(Inventory.instance.items[j]);
                    Debug.Log("Se removió el item " + Inventory.instance.items[j] + " del inventario");
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
}
