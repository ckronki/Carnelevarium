using UnityEngine; // Librería base de Unity para Vectors, Rays, Physics y GameObjects.
using UnityEngine.InputSystem; // Nueva librería de Input para registrar el mouse y la tecla R con precisión.

public class InventoryMouseManager : MonoBehaviour
{
    // --- VARIABLES EXPOSTAS EN EL INSPECTOR ---
    [Header("Configuración de Cámara")]
    public Camera inventoryCamera; // La cámara secundaria/específica que filma el inventario 3D.

    // --- VARIABLES INTERNAS PRIVADAS ---
    private InventorySystem inventorySystem; // Referencia al sistema central que valida los casilleros.
    private InventoryItemDrag itemSeleccionado; // Almacena el script de arrastre del ítem que tenemos agarrado actualmente.
    private Transform originalParent; // Recuerda quién era el padre del objeto antes de despegarlo (usualmente la cuadrícula).
    private float distanciaAlPlano; // Distancia matemática entre la cámara y el plano invisible de arrastre.
    private Vector3 offsetMundo; // Distancia de desfase entre el centro del objeto y el punto exacto donde el mouse hizo clic (evita que el objeto pegue un salto).

    // Start se ejecuta al iniciar el componente en escena
    void Start()
    {
        // Se conecta al Singleton central del inventario.
        inventorySystem = InventorySystem.Instance;

        // --- SISTEMA PREVENTIVO DE CÁMARA ---
        if (inventoryCamera == null) inventoryCamera = Camera.main; // Si no hay cámara asignada, busca la etiqueta "MainCamera".
        if (inventoryCamera == null) inventoryCamera = Object.FindFirstObjectByType<Camera>(); // Si sigue vacía, agarra la primera cámara que encuentre.
    }

    // Update procesa los tres estados del mouse (Click, Arrastre, Soltar) cuadro por cuadro
    void Update()
    {
        // CONTROL DE SEGURIDAD: Si no hay cámara activa o el inventario no cargó, frena el script para evitar errores en la consola.
        if (inventoryCamera == null || inventorySystem == null) return;

        // Lee la posición actual en píxeles de la flecha del mouse en la pantalla (Ej: X: 1920, Y: 1080).
        Vector2 mousePos = Mouse.current.position.ReadValue();

        // ==========================================
        // 1. ACCIÓN: AGARRAR (Primer frame del Click Izquierdo)
        // ==========================================
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // Convierte la posición 2D del mouse en un rayo matemático 3D que viaja desde el lente de la cámara hacia el fondo del escenario.
            Ray rayo = inventoryCamera.ScreenPointToRay(mousePos);

            // Lanza un Raycast físico. Filtra para ignorar objetos en la capa "Ignore Raycast" (como triggers o UI).
            if (Physics.Raycast(rayo, out RaycastHit hit, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
            {
                // Busca el script 'InventoryItemDrag' en el objeto impactado, en su padre, o en sus hijos (cobertura total de clicks).
                InventoryItemDrag dragScript = hit.transform.GetComponent<InventoryItemDrag>();
                if (dragScript == null) dragScript = hit.transform.GetComponentInParent<InventoryItemDrag>();
                if (dragScript == null) dragScript = hit.transform.GetComponentInChildren<InventoryItemDrag>();

                // Si encontramos un objeto que se puede arrastrar...
                if (dragScript != null)
                {
                    itemSeleccionado = dragScript; // Guardamos el ítem en la variable de mano.
                    originalParent = itemSeleccionado.transform.parent; // Guardamos su contenedor original.

                    // --- AJUSTES VISUALES DEL OBJETO AL LEVANTARLO ---
                    if (itemSeleccionado.item.contenedorVisual != null)
                    {
                        // Resetea su jerarquía local interna para asegurarse de que no haya desfaces visuales raros.
                        itemSeleccionado.item.contenedorVisual.SetParent(itemSeleccionado.transform, false);
                        itemSeleccionado.item.contenedorVisual.localPosition = Vector3.zero;

                        // Calcula el ángulo de rotación: si ya estaba girado lo mantiene a 90 grados, si no, a 0.
                        float anguloGiro = itemSeleccionado.item.rotado ? 90f : 0f;
                        // Mantiene el objeto acostado mirando hacia la cámara (90 en X) y aplica el giro en Z.
                        itemSeleccionado.item.contenedorVisual.localRotation = Quaternion.Euler(90f, 0f, anguloGiro);

                        // APLICACIÓN DE ESCALA PERSONALIZADA AL ARRASTRAR: Cambia su tamaño (ej: se hace un poco más chico o grande al flotar).
                        itemSeleccionado.item.contenedorVisual.localScale = itemSeleccionado.item.escalaAlArrastrar;
                    }

                    // Apaga el Collider propio del ítem para que el rayo del mouse no choque consigo mismo mientras lo movemos.
                    if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = false;

                    // Le avisa al sistema central que borre este objeto de las celdas de la matriz (así los casilleros quedan libres).
                    inventorySystem.LiberarSlotsDeItem(itemSeleccionado.item);

                    // Saca el objeto de la cuadrícula y lo vuelve hijo del sistema general para que se mueva con total libertad.
                    itemSeleccionado.transform.SetParent(inventorySystem.transform);

                    // --- MATEMÁTICA DEL PLANO DE ARRASTRE ---
                    // Crea un plano matemático invisible perpendicular a la cámara, posicionado a la altura del inventario.
                    Plane plano = new Plane(-inventoryCamera.transform.forward, originalParent.position);
                    if (plano.Raycast(rayo, out float distancia))
                    {
                        distanciaAlPlano = distancia; // Almacena la distancia exacta de profundidad.
                        Vector3 puntoImpacto = rayo.GetPoint(distanciaAlPlano - 0.1f); // Encuentra el punto exacto en el espacio 3D.
                        offsetMundo = itemSeleccionado.transform.position - puntoImpacto; // Calcula la distancia entre el centro del ítem y el click.
                    }
                }
            }
        }

        // ==========================================
        // 2. ACCIÓN: ARRASTRAR (Mantener pulsado el Click)
        // ==========================================
        if (itemSeleccionado != null)
        {
            // Actualiza constantemente el rayo y el plano invisible según se mueva el mouse por la pantalla.
            Ray rayoArrastre = inventoryCamera.ScreenPointToRay(mousePos);
            Plane planoArrastre = new Plane(-inventoryCamera.transform.forward, originalParent.position);

            if (planoArrastre.Raycast(rayoArrastre, out float distancia))
            {
                Vector3 puntoMundo = rayoArrastre.GetPoint(distancia - 0.1f); // Obtiene la nueva posición en el espacio.
                itemSeleccionado.transform.position = puntoMundo + offsetMundo; // Mueve el objeto sumando el offset para evitar saltos.
            }

            // --- ESCUCHA DE ROTACIÓN EN TIEMPO REAL ---
            if (Keyboard.current.rKey.wasPressedThisFrame)
            {
                itemSeleccionado.item.Rotate(); // Invierte las variables de ancho y alto en el script matemático.

                // Cambia la orientación visual del modelo 3D de inmediato para que el jugador vea el giro.
                if (itemSeleccionado.item.contenedorVisual != null)
                {
                    float anguloGiro = itemSeleccionado.item.rotado ? 90f : 0f;
                    itemSeleccionado.item.contenedorVisual.localRotation = Quaternion.Euler(90f, 0f, anguloGiro);
                }
            }

            // ==========================================
            // 3. ACCIÓN: SOLTAR (Soltar el Click Izquierdo)
            // ==========================================
            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                // Asegura que el collider siga apagado para realizar la última prueba de abajo sin interferencias.
                if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = false;

                Ray rayoSoltar = inventoryCamera.ScreenPointToRay(mousePos);

                // Lanza un rayo para ver sobre qué objeto del inventario estamos soltando el ítem.
                if (Physics.Raycast(rayoSoltar, out RaycastHit hitSlot, Mathf.Infinity, ~LayerMask.GetMask("Ignore Raycast")))
                {
                    Transform slot = hitSlot.transform; // Guardamos el casillero (Slot) detectado por el rayo.

                    // Si el objeto impactado pertenece efectivamente a la lista oficial de casilleros del inventario...
                    if (inventorySystem.slots.Contains(slot))
                    {
                        // Intenta colocar el ítem en ese casillero. El sistema validará si entra o si choca con otro objeto.
                        inventorySystem.PlaceItem(itemSeleccionado.item, slot);

                        // Reactiva su colisionador para que se pueda volver a agarrar en el futuro.
                        if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = true;

                        itemSeleccionado = null; // Vacía la mano (el ítem ya quedó guardado en su nuevo lugar).
                        return; // Finaliza la operación con éxito.
                    }
                }

                // --- CASO DE EMERGENCIA: SI LO SOLTASTE EN EL VACÍO O LUGAR INVÁLIDO ---
                if (itemSeleccionado.miCollider != null) itemSeleccionado.miCollider.enabled = true; // Reactiva su física.
                itemSeleccionado.RegresarAUltimoSlot(); // Invoca el método de rescate para regresar el objeto a donde estaba antes del click.
                itemSeleccionado = null; // Vacía la mano.
            }
        }
    }
}