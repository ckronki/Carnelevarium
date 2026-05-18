using UnityEngine;

public class InventoryItem : MonoBehaviour
{
    public ItemData data;
    public bool rotado = false;

    // Última posición válida
    public Transform lastSlot;

    // Rotar el ítem
    public void Rotate()
    {
        // Cambia el estado booleano de la rotación
        rotado = !rotado;

        // Actualiza la rotación visual en tiempo real mientras lo arrastras
        float anguloY = rotado ? 90f : 0f;
        transform.localRotation = Quaternion.Euler(90f, anguloY, 0f);

        Debug.Log($"[ITEM] Rotación actualizada. Rotado: {rotado} (Ángulo Y: {anguloY})");
    }

    public int GetWidth() => data.GetAncho(rotado);
    public int GetHeight() => data.GetAlto(rotado);

}
