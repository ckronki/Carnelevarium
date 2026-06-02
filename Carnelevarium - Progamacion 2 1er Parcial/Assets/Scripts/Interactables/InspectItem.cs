using UnityEngine;
using UnityEngine.InputSystem;

public class InspectItem : MonoBehaviour
{
    public static InspectItem Instance { get; private set; }
    public Camera mainCamera;

    private GameObject currentItem;
    private bool isInspecting = false;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (!isInspecting || currentItem == null) return;

        if (Mouse.current.leftButton.isPressed)
        {
            float rotX = Mouse.current.delta.x.ReadValue() * 100f * Time.unscaledDeltaTime;
            float rotY = -Mouse.current.delta.y.ReadValue() * 100f * Time.unscaledDeltaTime;
            currentItem.transform.Rotate(Vector3.up, rotX, Space.World);
            currentItem.transform.Rotate(Vector3.right, rotY, Space.World);
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseInspection();
        }
    }

    public void Inspect(GameObject prefab)
    {
        Debug.Log("Inspect() llamado");

        if (prefab == null || mainCamera == null)
        {
            Debug.LogError("Prefab o MainCamera no asignado.");
            return;
        }

        if (currentItem != null) CloseInspection();

        Vector3 spawnPos = mainCamera.transform.position + mainCamera.transform.forward * 2f;
        Quaternion spawnRot = Quaternion.identity;

        currentItem = Instantiate(prefab, spawnPos, spawnRot);
        Debug.Log("Prefab instanciado: " + currentItem.name);

        isInspecting = true;
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void CloseInspection()
    {
        if (currentItem != null) Destroy(currentItem);
        isInspecting = false;
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Debug.Log("Inspección cerrada");
    }

    public bool IsInspecting() => isInspecting;
}
