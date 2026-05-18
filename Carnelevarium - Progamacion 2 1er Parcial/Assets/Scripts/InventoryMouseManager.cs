using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryMouseManager : MonoBehaviour
{
    [Header("Configuración de Cámara")]
    public Camera inventoryCamera;

    private InventorySystem inventorySystem;
    private InventoryItemDrag itemSeleccionado;
    private Transform originalParent;
    private float distanciaAlPlano;
    private Vector3 offsetMundo;

    void Start()
    {
        inventorySystem = InventorySystem.Instance;

        // Si no se asignó en el inspector, la busca automáticamente
        if (inventoryCamera == null)
        {
            inventoryCamera = Camera.main;
        }

        if (inventoryCamera == null)
        {
            inventoryCamera = Object.FindFirstObjectByType<Camera>();
        }
    }

    void Update()
    {
        if (inventoryCamera == null || inventorySystem == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();

        // 1. DETECTAR CLIC INICIAL (AGARRAR)
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray rayo = inventoryCamera.ScreenPointToRay(mousePos);

            // Tiramos el rayo ignorando la capa del jugador (Ignore Raycast)
            if (Physics.Raycast(rayo, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
            {
                // Log de diagnóstico para saber qué estamos tocando exactamente
                Debug.Log($"[CLICK GLOBAL] El mouse impactó físicamente en: {hit.transform.name}");

                // Buscamos el script de arrastre en el objeto impactado
                InventoryItemDrag dragScript = hit.transform.GetComponent<InventoryItemDrag>();

                // Si no lo encuentra directo (porque tocó una malla hija), busca en los padres
                if (dragScript == null)
                {
                    dragScript = hit.transform.GetComponentInParent<InventoryItemDrag>();
                }

                // Si encontramos el script de arrastre, levantamos el ítem
                if (dragScript != null)
                {
                    itemSeleccionado = dragScript;
                    originalParent = itemSeleccionado.transform.parent;

                    // Desactivar el colisionador inmediatamente para que no estorbe al arrastrar sobre slots vacíos
                    if (itemSeleccionado.miCollider != null)
                    {
                        itemSeleccionado.miCollider.enabled = false;
                    }

                    // Liberamos los casilleros que ocupaba actualmente
                    inventorySystem.LiberarSlotsDeItem(itemSeleccionado.item);

                    // Mover temporalmente el objeto a la raíz del inventario para desvincularlo del slot
                    itemSeleccionado.transform.SetParent(inventorySystem.transform);

                    // Calcular la distancia física respecto a la cámara para mantener la profundidad 3D
                    Plane plano = new Plane(-inventoryCamera.transform.forward, originalParent.position);
                    if (plano.Raycast(rayo, out float distancia))
                    {
                        distanciaAlPlano = distancia;
                        // Forzamos un levísimo offset hacia la cámara (-0.1f) para evitar z-fighting visual
                        Vector3 puntoImpacto = rayo.GetPoint(distanciaAlPlano - 0.1f);
                        offsetMundo = itemSeleccionado.transform.position - puntoImpacto;
                    }
                }
            }
            else
            {
                Debug.Log("[CLICK GLOBAL] El rayo se fue al espacio infinito, no tocó ningún collider.");
            }
        }

        // 2. PROCESAR ARRASTRE ACTIVO
        if (itemSeleccionado != null)
        {
            Ray rayoArrastre = inventoryCamera.ScreenPointToRay(mousePos);
            Plane planoArrastre = new Plane(-inventoryCamera.transform.forward, originalParent.position);

            if (planoArrastre.Raycast(rayoArrastre, out float distancia))
            {
                Vector3 puntoMundo = rayoArrastre.GetPoint(distancia - 0.1f);
                itemSeleccionado.transform.position = puntoMundo + offsetMundo;
            }

            // Permitir rotación en tiempo real con la tecla R durante el arrastre
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                itemSeleccionado.item.Rotate();
            }

            // 3. DETECTAR CUANDO SE SUELTA EL CLIC (COLOCAR)
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // ¡CRUCIAL! Mantenemos el collider APAGADO un instante más 
                // para que el rayo lo atraviese por completo y logre tocar el slot del fondo
                if (itemSeleccionado.miCollider != null)
                {
                    itemSeleccionado.miCollider.enabled = false;
                }

                Ray rayoSoltar = inventoryCamera.ScreenPointToRay(mousePos);

                // Buscamos qué hay exactamente detrás del objeto
                if (Physics.Raycast(rayoSoltar, out RaycastHit hitSlot, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
                {
                    Transform slot = hitSlot.transform;
                    Debug.Log($"[SISTEMA] El mouse atravesó el ítem e impactó la base: {slot.name}");

                    if (inventorySystem.slots.Contains(slot))
                    {
                        // Intentamos colocarlo. Si PlaceItem tiene éxito, registrará el objeto.
                        inventorySystem.PlaceItem(itemSeleccionado.item, slot);

                        // Reactivamos el collider ahora que ya está guardado en su nuevo sitio
                        if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = true;

                        itemSeleccionado = null;
                        return;
                    }
                }

                // Si falló el Raycast o el inventario rechazó el ítem por falta de espacio matemático:
                Debug.LogWarning($"[SISTEMA] Colocación rechazada en {hitSlot.transform?.name}. Regresando a posición segura.");

                // Reactivamos el collider para que no se quede fantasma
                if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = true;

                itemSeleccionado.RegresarAUltimoSlot();
                itemSeleccionado = null;
            }
        }
    }
}