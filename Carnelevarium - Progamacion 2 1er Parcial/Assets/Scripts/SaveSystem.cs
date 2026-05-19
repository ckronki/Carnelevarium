using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    [Header("Referencias del Sistema")]
    public Inventory inventory; // Componente Inventory asignado desde el Inspector

    // Listas internas de control de la sesión actual
    private List<string> collectedObjectIDs = new List<string>();
    private string savePathFolder;

    void Awake()
    {
        savePathFolder = Application.persistentDataPath;
    }

    void Start()
    {
        // Al empezar el nivel, nos aseguramos de que el escenario sepa qué está recogido y qué no
        SincronizarObjetosDelSuelo();
    }

    [System.Serializable]
    public class SaveData
    {
        public List<string> items;
        public List<string> collectedObjectIDs;
        public float x;
        public float y;
        public float z;
    }

    public void RegistrarObjetoDestruido(string id)
    {
        if (string.IsNullOrEmpty(id)) return;

        if (!collectedObjectIDs.Contains(id))
        {
            collectedObjectIDs.Add(id);
            Debug.Log($"<color=purple>[SaveSystem]</color> Objeto registrado como recogido: {id}");
        }
    }

    public void SaveGame(int slot)
    {
        string fullPath = Path.Combine(savePathFolder, $"save_{slot}.json");
        SaveData data = new SaveData();

        data.items = new List<string>();

        // ====================================================================
        // MEJORA EXTREMA: GUARDADO DIRECTO DESDE LA CUADRÍCULA VISUAL
        // Si 'inventory' falló en registrar los strings, leemos la UI real
        // ====================================================================
        if (InventorySystem.Instance != null)
        {
            // Buscamos todos los ítems físicos reales dentro de la UI del inventario
            InventoryItem[] itemsEnMaletin = InventorySystem.Instance.GetComponentsInChildren<InventoryItem>(true);

            foreach (InventoryItem itemFisico in itemsEnMaletin)
            {
                if (itemFisico != null)
                {
                    // Limpiamos el nombre "(Clone)" que Unity añade automáticamente
                    string nombreLimpio = itemFisico.gameObject.name.Replace("(Clone)", "").Trim();

                    // Si tus prefabs terminan en "_inv", le removemos esa parte para guardar el nombre puro
                    if (nombreLimpio.EndsWith("_inv"))
                    {
                        nombreLimpio = nombreLimpio.Substring(0, nombreLimpio.Length - 4);
                    }

                    if (!data.items.Contains(nombreLimpio))
                    {
                        data.items.Add(nombreLimpio);
                    }
                }
            }
        }

        // Si por alguna razón la UI estaba cerrada pero la lista lógica tenía datos, los sumamos
        if (data.items.Count == 0 && inventory != null)
        {
            data.items = new List<string>(inventory.GetItems());
        }

        data.collectedObjectIDs = new List<string>(collectedObjectIDs);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(fullPath, json);

        Debug.Log($"<color=green>[SaveSystem]</color> ¡Guardado completado en Slot {slot}! Objetos en maletín: {data.items.Count}. Objetos retirados del suelo: {data.collectedObjectIDs.Count}");
    }

    public void LoadGame(int slot)
    {
        string fullPath = Path.Combine(savePathFolder, $"save_{slot}.json");

        if (!File.Exists(fullPath))
        {
            Debug.LogWarning($"[SaveSystem] No hay archivo para cargar en el Slot: {slot}");
            return;
        }

        string json = File.ReadAllText(fullPath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        // 1. Limpieza absoluta previa
        if (inventory != null)
        {
            inventory.Clear();
        }

        if (InventorySystem.Instance != null)
        {
            List<Transform> celdas = new List<Transform>(InventorySystem.Instance.slotMap.Keys);
            foreach (Transform celda in celdas)
            {
                InventorySystem.Instance.slotMap[celda] = null;
            }
        }

        // 2. Sobrescribimos las listas con los datos puros extraídos del JSON
        collectedObjectIDs = data.collectedObjectIDs ?? new List<string>();

        // 3. Recreamos los objetos en la cuadrícula
        if (data.items != null)
        {
            foreach (string itemName in data.items)
            {
                if (inventory != null)
                {
                    inventory.AddItem(itemName, true);
                }

                string nombreLimpio = itemName.ToLower().Trim();
                GameObject prefabInv = Resources.Load<GameObject>($"{nombreLimpio}_inv");

                if (prefabInv != null)
                {
                    GameObject nuevoItemVisual = Instantiate(prefabInv);
                    InventoryItem invItem = nuevoItemVisual.GetComponentInChildren<InventoryItem>();

                    if (invItem != null && InventorySystem.Instance != null)
                    {
                        bool pudoColocarse = InventorySystem.Instance.IntentarAñadirItemAuto(invItem);
                        if (!pudoColocarse)
                        {
                            Destroy(nuevoItemVisual);
                        }
                    }
                }
            }
        }

        // 4. Actualizamos el mapa físico
        SincronizarObjetosDelSuelo();
        Debug.Log($"<color=yellow>[SaveSystem]</color> ¡Carga exitosa del Slot {slot}!");
    }

    private void SincronizarObjetosDelSuelo()
    {
        PickupItem[] objetosEnEscena = FindObjectsOfType<PickupItem>(true);

        foreach (PickupItem item in objetosEnEscena)
        {
            if (item.gameObject.scene.name == null) continue; // Ignorar prefabs del proyecto

            // REGLA DE ORO DE REACTIVACIÓN:
            // Si el ID está en el JSON -> Se desactiva (ya lo tienes).
            // Si NO está en el JSON -> ¡Se activa obligatoriamente!
            bool yaRecogido = collectedObjectIDs.Contains(item.objectID);

            item.gameObject.SetActive(!yaRecogido);

            if (item.flashlightObject != null)
            {
                item.flashlightObject.SetActive(!yaRecogido);
            }
        }
    }
}