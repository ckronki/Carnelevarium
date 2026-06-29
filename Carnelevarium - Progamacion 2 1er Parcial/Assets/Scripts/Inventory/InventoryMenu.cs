using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryMenu : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] GameObject inventoryCanvas; // Canvas del inventario
    [SerializeField] GameObject gridInventory;   // Panel con los slots
    [SerializeField] MonoBehaviour cameraController; // Script de cámara
    [SerializeField] MonoBehaviour playerController; // Script de movimiento del jugador

    private bool isOpen = false;

    void Start()
    {
        if (inventoryCanvas != null)
            inventoryCanvas.SetActive(false);
        if (gridInventory != null)
            gridInventory.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    private void ToggleInventory()
    {
        isOpen = !isOpen;

        if (isOpen)
            AbrirInventario();
        else
            CerrarInventario();
    }

    private void AbrirInventario()
    {
        Time.timeScale = 0f;

        if (inventoryCanvas != null) inventoryCanvas.SetActive(true);
        if (gridInventory != null) gridInventory.SetActive(true);

        if (cameraController != null)
        {
            var camController = cameraController.GetComponent<CameraController>();
            if (camController != null)
                camController.enabled = false;
        }

        if (playerController != null) playerController.enabled = false;
        cameraController.GetComponent<CameraController>().LockCamera();

        Debug.Log("<color=green>[Inventario]</color> Inventario abierto.");
    }

    private void CerrarInventario()
    {
        Time.timeScale = 1f;

        if (inventoryCanvas != null) inventoryCanvas.SetActive(false);
        if (gridInventory != null) gridInventory.SetActive(false);

        if (cameraController != null)
        {
            var camController = cameraController.GetComponent<CameraController>();
            if (camController != null)
                camController.enabled = true;
        }

        cameraController.GetComponent<CameraController>().UnlockCamera();
        if (playerController != null) playerController.enabled = true;

        Debug.Log("<color=yellow>[Inventario]</color> Inventario cerrado.");
    }

}
