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

    public void SetItem(GameObject itemPrefab, GameObject worldReference)
    {
        // Instanciamos el objeto en la Layer "Inspect"
        currentItem = Instantiate(itemPrefab);
        currentItem.layer = LayerMask.NameToLayer("Inspect");

        // Lo posicionamos frente a la cámara secundaria
        currentItem.transform.position = inspectCamera.transform.position + inspectCamera.transform.forward * 2f;
        currentItem.transform.rotation = Quaternion.identity;
        currentItem.transform.localScale = Vector3.one;

        // Desactivar collider
        Collider col = currentItem.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        worldItem = worldReference;
        StartInspect();
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

    void EndInspect()
    {
        isInspecting = false;
        Time.timeScale = 1f; // Reanuda el juego

        // Reactivar controles del jugador
        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl != null) camCtrl.enabled = true;

        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;

        // Ocultar cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (currentItem != null) Destroy(currentItem);
        if (worldItem != null) Destroy(worldItem);

        Debug.Log("Modo inspección DESACTIVADO");
    }

    public bool IsInspecting() => isInspecting;
}
