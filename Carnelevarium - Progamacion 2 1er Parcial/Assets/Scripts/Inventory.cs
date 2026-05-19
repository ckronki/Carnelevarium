using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // --- VARIABLES INTERNAS PRIVADAS ---
    public List<string> items = new List<string>();

    public static Inventory instance;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }

    // Se añade el parámetro opcional 'esPorCarga'. Si no se especifica, por defecto es 'false'.
    public void AddItem(string itemName, bool esPorCarga = false)
    {
        items.Add(itemName);

        // Si el ítem NO proviene del archivo de carga, significa que se acaba de recoger/agregar en la sesión actual
        if (!esPorCarga)
        {
            Debug.Log($"<color=green>[Inventario]</color> ¡Nuevo Item agregado en tiempo real!: {itemName} (Guardado en lista local).");
        }
        else
        {
            Debug.Log($"<color=yellow>[Inventario - Carga]</color> Item restaurado desde el archivo de guardado: {itemName}");
        }
    }

    public bool HasItem(string itemName)
    {
        return items.Contains(itemName);
    }

    public void Clear()
    {
        items.Clear();
    }

    public List<string> GetItems()
    {
        return items;
    }
}