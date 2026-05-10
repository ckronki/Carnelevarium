using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    private List<string> items = new List<string>();

    public void AddItem(string itemName)
    {
        items.Add(itemName);
        Debug.Log("Item agregado: " + itemName);
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }
}