using UnityEngine; // Librería base de Unity necesaria para heredar de MonoBehaviour, usar Colliders y Debugs.

public class InventoryItemDrag : MonoBehaviour
{
    // --- VARIABLES PÚBLICAS REQUERIDAS (OCULTAS EN EL INSPECTOR) ---
    // [HideInInspector] evita que estas variables saturen la interfaz del Inspector de Unity, ya que se llenan solas por código.

    [HideInInspector] public InventoryItem item; // Referencia al componente hermano 'InventoryItem' para conocer sus dimensiones y rotación.
    [HideInInspector] public Collider miCollider; // Referencia al colisionador (BoxCollider, etc.) de este objeto para poder prenderlo/apagarlo durante el arrastre.

    // --- VARIABLES INTERNAS PRIVADAS ---
    private InventorySystem inventorySystem; // Almacena la referencia al sistema central del inventario (el tablero que gestiona las celdas).

    // Start se ejecuta automáticamente en el primer fotograma en que el objeto despierta en la escena
    void Start()
    {
        // Busca y guarda de forma automática el componente 'InventoryItem' alojado en este mismo GameObject.
        item = GetComponent<InventoryItem>();

        // Busca y guarda el Collider fijado en este mismo GameObject (esencial para que el mouse pueda hacer "Raycast" y detectarlo).
        miCollider = GetComponent<Collider>();

        // Se conecta al Singleton 'Instance' del InventorySystem para poder enviarle órdenes directamente sin necesidad de arrastrarlo manualmente.
        inventorySystem = InventorySystem.Instance;
    }

    // Método de rescate: se activa cuando el jugador cancela el arrastre o intenta soltar el ítem en un área prohibida
    public void RegresarAUltimoSlot()
    {
        // FILTRO DE SEGURIDAD: Verifica que el ítem exista, que recuerde su última posición válida (lastSlot) y que el sistema central esté activo.
        if (item != null && item.lastSlot != null && inventorySystem != null)
        {
            // Le ordena al InventorySystem que ejecute 'ForzarColocacionFísica', reubicando los gráficos del ítem exactamente sobre el casillero guardado.
            inventorySystem.ForzarColocacionFísica(item, item.lastSlot);
        }
    }
}