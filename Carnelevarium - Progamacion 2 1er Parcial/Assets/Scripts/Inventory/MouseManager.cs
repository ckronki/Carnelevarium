using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MouseManager : MonoBehaviour
{
    [Header("Referencias")]
    public Camera cam;
    public GraphicRaycaster uiRaycaster;
    public EventSystem eventSystem;

    private InventoryItem itemSeleccionado;
    private ItemAnchorManager anchorManager;

    void Update()
    {
        if (Time.timeScale == 0f) Physics.autoSimulation = true;

        if (Input.GetMouseButtonDown(0))
        {
            if (itemSeleccionado == null)
                IntentarSeleccionarItem();
            else
                IntentarSoltarItem();
        }

        if (itemSeleccionado != null && Input.GetKeyDown(KeyCode.R))
        {
            itemSeleccionado.Rotate();
        }

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
                anchorManager.LiberarSlots(); // libera los slots previos
                item.transform.SetParent(null);
                item.transform.localScale = item.escalaAlArrastrar;
                Debug.Log($"[Mouse] Ítem '{item.name}' seleccionado.");
            }
        }
    }

    void MoverItemConMouse()
    {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            uiRaycaster.GetComponent<RectTransform>(),
            Input.mousePosition,
            cam,
            out pos
        );

        itemSeleccionado.transform.localPosition = pos;
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
