using UnityEngine;

using UnityEngine.InputSystem;



public class InspectItem : MonoBehaviour

{

    [Header("Zoom")]

    public float minScale = 0.5f;   // Zoom mínimo

    public float maxScale = 2f;     // Zoom máximo

    public float zoomSpeed = 0.1f;  // Velocidad de zoom



    [Header("Cámara de inspección")]

    public Camera inspectCamera;    // Cámara secundaria que renderiza al RawImage



    private GameObject currentItem; // Instancia del objeto inspeccionado

    private GameObject worldItem;   // Referencia al objeto original

    private bool isInspecting = false;

    private GameObject itemPrefabRef; // Guardar referencia del prefab original
    private PickupItem currentPickupRef; // Guardar script que originó la llamada
    private string currentID;

    public Animator animator;

    void Update()

    {

        if (!isInspecting || currentItem == null) return;



        // Rotación con mouse

        if (Mouse.current.leftButton.isPressed)

        {

            float rotX = Mouse.current.delta.x.ReadValue();

            float rotY = Mouse.current.delta.y.ReadValue();



            currentItem.transform.Rotate(Vector3.up, -rotX, Space.World);

            currentItem.transform.Rotate(Vector3.right, rotY, Space.World);

        }



        // Zoom con scroll (ajusta escala en vez de posición)

        float scroll = Mouse.current.scroll.ReadValue().y;

        if (scroll != 0)

        {

            float scaleChange = 1f + scroll * zoomSpeed;



            currentItem.transform.localScale *= scaleChange;



            // Clamp de escala

            float clampedScale = Mathf.Clamp(currentItem.transform.localScale.x, minScale, maxScale);

            currentItem.transform.localScale = Vector3.one * clampedScale;

        }



        // Salir con Escape

        if (Keyboard.current.escapeKey.wasPressedThisFrame)

        {

            EndInspect();

        }

    }



    public void SetItem(GameObject itemPrefab, GameObject worldReference, string id)
    {
        itemPrefabRef = itemPrefab;
        currentID = id;
        currentPickupRef = worldReference != null ? worldReference.GetComponent<PickupItem>() : null;

        currentItem = Instantiate(itemPrefab);
        currentItem.layer = LayerMask.NameToLayer("Inspect");

        // Desactivar lógica de arrastre del inventario mientras se inspecciona en la caja flotante
        if (currentItem.TryGetComponent(out InventoryItemDrag drag)) drag.enabled = false;

        currentItem.transform.position = inspectCamera.transform.position + inspectCamera.transform.forward * 2f;
        currentItem.transform.rotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        Collider col = currentItem.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        worldItem = worldReference;
        StartInspect();
    }

    // Modifica la parte final de tu EndInspect original por esto:
    void EndInspect()
    {
        isInspecting = false;
        Time.timeScale = 1f;

        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl != null) camCtrl.enabled = true;

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentItem != null) Destroy(currentItem);

        // INTENTO DE ENTRADA AL INVENTARIO AL CERRAR LA INSPECCIÓN
        if (InventorySystem.Instance != null && itemPrefabRef != null)
        {
            GameObject contenedorNuevo = Instantiate(itemPrefabRef);
            InventoryItem invItem = contenedorNuevo.GetComponent<InventoryItem>();

            if (InventorySystem.Instance.IntentarAñadirItemAuto(invItem))
            {
                Debug.Log("¡Objeto inspeccionado y guardado con éxito en la cuadrícula!");
                if (currentPickupRef != null) currentPickupRef.GuardadoExitosoEnInventario();
            }
            else
            {
                Debug.Log("No queda espacio en los bloques del inventario para este ítem.");
                Destroy(contenedorNuevo);
                if (worldItem != null) worldItem.SetActive(true); // Lo dejamos en el suelo por falta de espacio
            }
        }

        Debug.Log("Modo inspección DESACTIVADO");
    }


    void StartInspect()

    {

        isInspecting = true;

        Time.timeScale = 0f; // Pausa el juego



        // Bloquear controles del jugador

        CameraController camCtrl = FindObjectOfType<CameraController>();

        if (camCtrl != null) camCtrl.enabled = false;



        PlayerInput playerInput = FindObjectOfType<PlayerInput>();

        if (playerInput != null) playerInput.enabled = false;



        // Mostrar cursor

        Cursor.lockState = CursorLockMode.None;

        Cursor.visible = true;



        Debug.Log("Modo inspección ACTIVADO");



        animator.SetTrigger("TakingItem");

    }



   


    public bool IsInspecting() => isInspecting;

}