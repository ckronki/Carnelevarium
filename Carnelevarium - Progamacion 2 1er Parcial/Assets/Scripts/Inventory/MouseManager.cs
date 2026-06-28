using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    [Header("Configuración del Mouse")]
    public Camera cam; // Cámara principal
    private InventoryItem itemSeleccionado;
    private ItemAnchorManager anchorManager;

    void Update()
    {
        // Detectar clic izquierdo para agarrar o soltar
        if (Input.GetMouseButtonDown(0))
        {
            if (itemSeleccionado == null)
                IntentarSeleccionarItem();
            else
                IntentarSoltarItem();
        }

        // Detectar tecla R para rotar el ítem
        if (itemSeleccionado != null && Input.GetKeyDown(KeyCode.R))
        {
            itemSeleccionado.Rotate();
        }

        // Si hay un ítem seleccionado, seguir el mouse
        if (itemSeleccionado != null)
        {
            MoverItemConMouse();
        }
    }

    void IntentarSeleccionarItem()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            InventoryItem item = hit.collider.GetComponent<InventoryItem>();
            if (item != null)
            {
                itemSeleccionado = item;
                anchorManager = item.GetComponent<ItemAnchorManager>();
                item.transform.SetParent(null); // Lo sacamos del slot temporalmente
                item.transform.localScale = item.escalaAlArrastrar;
                Debug.Log($"<color=yellow>[Mouse]</color> Ítem '{item.name}' seleccionado.");
            }
        }
    }

    void MoverItemConMouse()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            itemSeleccionado.transform.position = hit.point + Vector3.up * 0.5f;
        }
    }

    void IntentarSoltarItem()
    {
        if (anchorManager != null && anchorManager.PuedeColocarse())
        {
            anchorManager.ColocarItem();
            itemSeleccionado.transform.localScale = itemSeleccionado.escalaEnInventario;
            itemSeleccionado = null;
            anchorManager = null;
        }
        else
        {
            Debug.Log("<color=red>[Mouse]</color> No se puede colocar el ítem aquí.");
        }
    }
}
