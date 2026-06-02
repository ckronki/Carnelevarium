using UnityEngine;
using UnityEngine.InputSystem;

public class ItemInspector : MonoBehaviour
{
    [Header("Referencias")]
    public Camera inspectCamera;       // Cámara secundaria
    public Transform inspectPoint;     // Empty delante de la cámara

    private GameObject currentItem;
    private bool isInspecting = false;

    void Update()
    {
        if (!isInspecting || currentItem == null) return;

        // Rotación con mouse
        if (Mouse.current.leftButton.isPressed)
        {
            float rotX = Mouse.current.delta.x.ReadValue() * 100f * Time.unscaledDeltaTime;
            float rotY = -Mouse.current.delta.y.ReadValue() * 100f * Time.unscaledDeltaTime;
            currentItem.transform.Rotate(Vector3.up, rotX, Space.World);
            currentItem.transform.Rotate(Vector3.right, rotY, Space.World);
        }

        // Cerrar con Escape
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInspection();
        }
    }

    public void Inspect(GameObject prefab)
    {
        if (prefab == null || inspectPoint == null)
        {
            Debug.LogError("Prefab o InspectPoint no asignado.");
            return;
        }

        if (currentItem != null) CloseInspection();

        currentItem = Instantiate(prefab, inspectPoint.position, inspectPoint.rotation);
        currentItem.transform.SetParent(inspectPoint, true);

        isInspecting = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Modo inspección ACTIVADO con " + currentItem.name);
    }

    public void CloseInspection()
    {
        if (currentItem != null) Destroy(currentItem);

        isInspecting = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        Debug.Log("Modo inspección CERRADO");
    }

    public bool IsInspecting() => isInspecting;
}
