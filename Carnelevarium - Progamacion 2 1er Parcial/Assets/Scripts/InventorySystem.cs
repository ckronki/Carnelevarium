using UnityEngine;
using System.Collections.Generic;

public class InventorySystem : MonoBehaviour
{
    [Header("Slots del Inventario")]
    public List<Transform> slots;
    public int columnas = 4;

    private Dictionary<Transform, InventoryItem> slotMap = new Dictionary<Transform, InventoryItem>();
    public static InventorySystem Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    public bool IntentarAñadirItemAuto(InventoryItem item)
    {
        // Busca de forma inteligente celdas que cumplan con la matriz completa (Ancho x Alto)
        foreach (Transform slot in slots)
        {
            List<Transform> celdasRequeridas = GetSlotsForItem(slot, item.GetWidth(), item.GetHeight());
            if (CanPlaceItem(celdasRequeridas))
            {
                PlaceItem(item, slot);
                return true;
            }
        }
        return false;
    }

    public void PlaceItem(InventoryItem item, Transform startSlot)
    {
        List<Transform> requiredSlots = GetSlotsForItem(startSlot, item.GetWidth(), item.GetHeight());

        if (CanPlaceItem(requiredSlots))
        {
            // 1. Limpiar registros antiguos de este ítem específico para no duplicar espacio
            LiberarSlotsDeItem(item);

            // 2. Ocupar los nuevos slots
            foreach (Transform slot in requiredSlots)
            {
                slotMap[slot] = item;
            }

            // 3. Emparejar físicamente en el espacio 3D
            ForzarColocacionFísica(item, startSlot);
        }
        else
        {
            // Revertir
            InventoryItemDrag dragScript = item.GetComponent<InventoryItemDrag>();
            if (dragScript != null) dragScript.RegresarAUltimoSlot();
        }
    }

    public void ForzarColocacionFísica(InventoryItem item, Transform targetSlot)
    {
        // 1. Emparentamos de forma limpia
        item.transform.SetParent(targetSlot);

        // 2. ¡SOLUCIÓN AL ESTIRAMIENTO!: Forzamos la escala global (del mundo) a (1,1,1)
        // Al usar un bucle 'while', nos aseguramos de limpiar la escala de CUALQUIER hijo visual interno.
        item.transform.localScale = Vector3.one;

        // Con este truco anulamos la deformación que le hereda el padre en el espacio global
        Vector3 escalaMundoActual = item.transform.lossyScale;
        item.transform.localScale = new Vector3(
            1f / (escalaMundoActual.x == 0 ? 1 : escalaMundoActual.x),
            1f / (escalaMundoActual.y == 0 ? 1 : escalaMundoActual.y),
            1f / (escalaMundoActual.z == 0 ? 1 : escalaMundoActual.z)
        );

        // 3. Posicionamiento ligeramente al frente para evitar z-fighting
        item.transform.localPosition = new Vector3(0f, 0f, -0.2f);

        // 4. Apagar fuerzas físicas del Rigidbody del ítem (si lo tiene)
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true;
            itemRb.linearVelocity = Vector3.zero;
            itemRb.angularVelocity = Vector3.zero;
        }

        // 5. Volvemos el Box Collider un Trigger
        BoxCollider boxCol = item.GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            boxCol.isTrigger = true;
            boxCol.center = Vector3.zero;
            boxCol.size = Vector3.one;
        }

        // 6. Mantener la rotación alineada (X=90, Y=0 o 90, Z=0)
        float anguloY = item.rotado ? 90f : 0f;
        item.transform.localRotation = Quaternion.Euler(90f, anguloY, 0f);

        item.lastSlot = targetSlot;

        // Volver a registrar los slots
        List<Transform> requiredSlots = GetSlotsForItem(targetSlot, item.GetWidth(), item.GetHeight());
        foreach (Transform slot in requiredSlots)
        {
            slotMap[slot] = item;
        }
    }

    public void LiberarSlotsDeItem(InventoryItem item)
    {
        // Creamos una lista temporal para guardar las llaves a liberar y evitar errores de modificación durante el bucle
        List<Transform> llavesALiberar = new List<Transform>();

        foreach (var pair in slotMap)
        {
            if (pair.Value == item)
            {
                llavesALiberar.Add(pair.Key);
            }
        }

        // Limpiamos los slots de forma segura
        foreach (Transform slot in llavesALiberar)
        {
            slotMap[slot] = null; // O remuévelo si usas un sistema de ocupación booleano
        }
    }
    public bool HasItemInSlot(Transform slot) => slotMap.ContainsKey(slot);

    bool CanPlaceItem(List<Transform> requiredSlots)
    {
        foreach (Transform slot in requiredSlots)
        {
            if (slot == null) return false;
            if (slotMap.ContainsKey(slot)) return false;
        }
        return true;
    }

    List<Transform> GetSlotsForItem(Transform startSlot, int width, int height)
    {
        List<Transform> result = new List<Transform>();

        int index = slots.IndexOf(startSlot);
        if (index == -1) return result;

        int filaInicial = index / columnas;
        int colInicial = index % columnas;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int actualFila = filaInicial + y;
                int actualCol = colInicial + x;

                // Controlar que el objeto no se desborde horizontalmente hacia la siguiente línea
                if (actualCol >= columnas || actualIndexFueraDeRango(actualFila, actualCol))
                {
                    result.Add(null);
                    continue;
                }

                int targetIndex = actualFila * columnas + actualCol;
                if (targetIndex >= 0 && targetIndex < slots.Count)
                {
                    result.Add(slots[targetIndex]);
                }
                else
                {
                    result.Add(null);
                }
            }
        }
        return result;
    }

    private bool actualIndexFueraDeRango(int f, int c) => f * columnas + c >= slots.Count;

    public void RemoveItem(InventoryItem item)
    {
        LiberarSlotsDeItem(item);
        Destroy(item.gameObject);
    }
}