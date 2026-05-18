using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory3DUI : MonoBehaviour
{
    [Header("UI Feedback")]
    public TMP_Text inventoryMessage;

    [Header("Configuración del Inventario")]
    [Tooltip("Arrastra aquí el PREFAB 3D de tu inventario desde la carpeta de Assets")]
    public GameObject inventoryPrefab;

    public Transform inventorySpotA;
    public Transform inventorySpotB;

    [Header("Cámara")]
    public Camera playerCamera;
    public Transform playerCameraSpot;
    private CameraController camController;

    [Header("Player")]
    public GameObject playerSkin;
    // NUEVO: Agregamos la referencia al script de control del personaje
    public Player playerScript;

    [Header("Colisiones")]
    public LayerMask obstacleMask;

    // Esta será la referencia interna al objeto real que operará en el mundo
    private GameObject activeInventoryInstance;
    private bool isOpen = false;
    private bool isTransitioning = false;

    void Start()
    {
        camController = playerCamera.GetComponent<CameraController>();

        // Cerciorarse de que el tiempo inicie normal pase lo que pase
        Time.timeScale = 1f;

        // INSTANCIACIÓN AUTOMÁTICA AL INICIAR
        if (inventoryPrefab != null)
        {
            // Creamos el inventario en una esquina lejana del mapa para que empiece a funcionar en segundo plano
            activeInventoryInstance = Instantiate(inventoryPrefab, new Vector3(0, -500f, 0), Quaternion.identity);

            // Lo apagamos temporalmente para que no estorbe visualmente
            activeInventoryInstance.SetActive(false);
        }
        else
        {
            Debug.LogError("¡Falta asignar el Prefab del Inventario en el componente Inventory3DUI!");
        }
    }

    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    bool CanSpawnAt(Transform spot)
    {
        Collider spotCollider = spot.GetComponent<Collider>();
        if (spotCollider == null) return true;

        Vector3 halfExtents = spotCollider.bounds.extents;
        Vector3 center = spotCollider.bounds.center;

        Collider[] hits = Physics.OverlapBox(center, halfExtents, spot.rotation, obstacleMask);
        foreach (Collider hit in hits)
        {
            if (hit.transform != spot && !hit.transform.IsChildOf(transform)) return false;
        }
        return true;
    }

    void ToggleInventory()
    {
        if (isTransitioning || activeInventoryInstance == null) return;

        isOpen = !isOpen;

        // Cancelamos corrutinas de movimiento previas para evitar que choquen si spameas el Tab
        StopAllCoroutines();

        if (isOpen)
        {
            Transform chosenSpot = null;

            if (CanSpawnAt(inventorySpotA)) chosenSpot = inventorySpotA;
            else if (CanSpawnAt(inventorySpotB)) chosenSpot = inventorySpotB;

            if (chosenSpot == null)
            {
                StartCoroutine(ShowMessage("No hay espacio para abrir el inventario", 2f));
                isOpen = false;
                return;
            }

            if (inventoryMessage != null) inventoryMessage.gameObject.SetActive(false);
            if (playerSkin != null) playerSkin.SetActive(false);

            // 1. Apagamos los controles de movimiento del Player
            if (playerScript != null)
            {
                playerScript.enabled = false;
                if (playerScript.animator != null) playerScript.animator.SetBool("isWalking", false);
                Rigidbody playerRb = playerScript.GetComponent<Rigidbody>();
                if (playerRb != null) playerRb.linearVelocity = Vector3.zero;
            }

            // 2. Apagamos el control del mouse de la cámara ANTES de moverla
            if (camController != null)
            {
                camController.enabled = false;
            }

            // 3. Posicionamos y encendemos el inventario
            activeInventoryInstance.transform.position = chosenSpot.position;
            activeInventoryInstance.transform.rotation = chosenSpot.rotation;
            activeInventoryInstance.SetActive(true);

            // 4. Buscamos el punto de la cámara e iniciamos la transición suave
            Transform camPoint = activeInventoryInstance.transform.Find("CameraPoint");
            if (camPoint != null)
            {
                StartCoroutine(MoveCamera(camPoint.position, camPoint.rotation));
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            // AL CERRAR
            activeInventoryInstance.SetActive(false);

            if (playerSkin != null) playerSkin.SetActive(true);

            // 1. Iniciamos la corrutina de regreso al spot del jugador
            StartCoroutine(MoveCamera(playerCameraSpot.position, playerCameraSpot.rotation));

            // 2. ¡EL TRUCO MAESTRO!: Sincronizamos los ángulos acumulados del script 
            // con la rotación que tiene el cuello del jugador (playerCameraSpot)
            if (camController != null)
            {
                camController.SincronizarAngulos(playerCameraSpot.rotation);
                camController.enabled = true; // Volvemos a encender el script
            }

            // 3. Devolvemos el movimiento al jugador
            if (playerScript != null) playerScript.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (inventoryMessage != null) inventoryMessage.gameObject.SetActive(false);
        }
    }
    IEnumerator ShowMessage(string text, float duration)
    {
        if (inventoryMessage == null) yield break;
        inventoryMessage.text = text;
        inventoryMessage.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        inventoryMessage.gameObject.SetActive(false);
    }

    IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        isTransitioning = true;
        float duration = 0.4f;
        float elapsed = 0f;

        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;

            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null;
        }

        playerCamera.transform.position = targetPos;
        playerCamera.transform.rotation = targetRot;
        isTransitioning = false;
    }
}