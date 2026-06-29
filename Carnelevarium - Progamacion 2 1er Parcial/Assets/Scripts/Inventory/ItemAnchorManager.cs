using UnityEngine;
using System.Collections.Generic;

public class ItemAnchorManager : MonoBehaviour
{
    [Header("Anchor Points del ítem")]
    public List<AnchorPoint> anchorPoints = new List<AnchorPoint>();

    void Awake()
    {
        // 🔹 Detecta automáticamente todos los AnchorPoints hijos
        anchorPoints.Clear();
        AnchorPoint[] encontrados = GetComponentsInChildren<AnchorPoint>();
        foreach (AnchorPoint a in encontrados)
        {
            anchorPoints.Add(a);
            a.itemPadre = GetComponent<InventoryItem>(); // asegura referencia al ítem padre
        }

        Debug.Log($"<color=cyan>[AnchorManager]</color> Detectados {anchorPoints.Count} AnchorPoints en '{gameObject.name}'.");
    }

    public bool PuedeColocarse()
    {
        foreach (AnchorPoint anchor in anchorPoints)
        {
            if (anchor.slotDetectado == null || anchor.slotDetectado.ocupado)
                return false;
        }
        return true;
    }

    public void ColocarItem()
    {
        if (!PuedeColocarse()) return;

        foreach (AnchorPoint anchor in anchorPoints)
        {
            anchor.slotDetectado.ocupado = true;
            anchor.slotDetectado.itemActual = anchor.itemPadre;
        }

        // 🔹 Centra el ítem en el primer slot detectado
        Transform primerSlot = anchorPoints[0].slotDetectado.transform;
        transform.SetParent(primerSlot.parent);
        transform.position = primerSlot.position;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"<color=green>[Inventario]</color> Ítem '{gameObject.name}' colocado correctamente ocupando {anchorPoints.Count} slots.");
    }

    public void LiberarSlots()
    {
        foreach (AnchorPoint anchor in anchorPoints)
        {
            if (anchor.slotDetectado != null)
            {
                anchor.slotDetectado.ocupado = false;
                anchor.slotDetectado.itemActual = null;
            }
        }
    }
}
