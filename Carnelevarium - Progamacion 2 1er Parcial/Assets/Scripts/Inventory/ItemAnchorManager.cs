using UnityEngine;
using System.Collections.Generic;

public class ItemAnchorManager : MonoBehaviour
{
    [Header("Anchor Points del ítem")]
    public List<AnchorPoint> anchorPoints = new List<AnchorPoint>();

    public bool PuedeColocarse()
    {
        foreach (AnchorPoint anchor in anchorPoints)
        {
            if (anchor.slotDetectado == null || anchor.slotDetectado.ocupado)
                return false; // Si algún punto no tiene slot o está ocupado, no se puede colocar
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

        Debug.Log($"<color=green>[Inventario]</color> Ítem '{gameObject.name}' colocado correctamente.");
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
