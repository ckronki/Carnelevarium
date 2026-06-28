using UnityEngine;
using System.Collections.Generic; // Requerido para usar listas dinámicas y estructuras Dictionary (mapas de datos).

public class InventorySystem : MonoBehaviour
{
    // --- PATRÓN SINGLETON ---
    // Permite que cualquier otro script (como MouseManager) acceda a este sistema usando 'InventorySystem.Instance' sin buscarlo.
    public static InventorySystem Instance { get; private set; }

    [Header("Configuración de la Cuadrícula")]
    public List<Transform> slots = new List<Transform>(); // Lista ordenada de todos los GameObjects que actúan como casilleros físicos en la UI/Mundo.
    public int gridWidth = 5;  // Columnas máximas de la cuadrícula (Ancho).
    public int gridHeight = 5; // Filas máximas de la cuadrícula (Alto).

    // Diccionario clave-valor que asocia cada Casillero (Transform) con el Objeto (InventoryItem) que lo está pisando. Si vale 'null', está libre.
    public Dictionary<Transform, InventoryItem> slotMap = new Dictionary<Transform, InventoryItem>();

    // Awake se ejecuta antes que el Start al cargar la escena
    void Awake()
    {
        // Configuración y validación del Singleton para evitar que existan dos inventarios duplicados al mismo tiempo.
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        InicializarContenedor(); // Prepara el mapa de datos.
    }

    // Registra todos los slots de la lista en el diccionario inicializándolos como vacíos (null)
    private void InicializarContenedor()
    {
        foreach (Transform slot in slots)
        {
            if (slot != null && !slotMap.ContainsKey(slot))
            {
                slotMap[slot] = null; // El casillero empieza limpio y disponible.
            }
        }
    }

    // Método automático para meter un ítem recolectado del suelo en el primer hueco libre que encaje
    public bool IntentarAñadirItemAuto(InventoryItem item)
    {
        // FILTROS DE SEGURIDAD: Evitan crasheos si el objeto viene dañado o mal configurado desde el Inspector.
        if (item == null)
        {
            Debug.LogError("[INVENTARIO] ¡El ítem que intentas añadir es NULL!");
            return false;
        }

        if (item.contenedorVisual == null)
        {
            Debug.LogError($"[INVENTARIO] El objeto '{item.gameObject.name}' no tiene asignado el Contenedor Visual en el Inspector.");
            return false;
        }

        // Recorre todos los casilleros de la cuadrícula uno por uno simulando que son la esquina superior izquierda del ítem
        foreach (Transform slot in slots)
        {
            // Si el ítem entra perfectamente en este slot y sus casilleros adyacentes...
            if (CanPlaceItem(item, slot))
            {
                PlaceItem(item, slot); // Lo posiciona físicamente y reserva sus bloques en la matriz.
                return true; // Retorna éxito y corta el bucle.
            }
        }
        return false; // Retorna falso si recorrió todo el bolso y no encontró espacio suficiente.
    }

    // Algoritmo matemático para comprobar si un ítem cabe en un slot específico sin salirse de los bordes ni chocar con otros
    public bool CanPlaceItem(InventoryItem item, Transform targetSlot)
    {
        // Obtiene el índice numérico (0, 1, 2...) del slot seleccionado dentro de la lista global.
        int startIndex = slots.IndexOf(targetSlot);
        if (startIndex == -1) return false; // Seguridad: si el slot no pertenece a este inventario, rebota la acción.

        // --- TRUCO MATEMÁTICO: Conversión de Índice Lineal a Coordenadas (X, Y) ---
        int startX = startIndex % gridWidth;  // El residuo (%) da la columna exacta en la cuadrícula.
        int startY = startIndex / gridWidth;  // La división entera (/) da la fila exacta en la cuadrícula.

        // Lee el tamaño actual que el ítem requiere (considerando si el jugador lo rotó con la R).
        int itemW = item.GetWidth();
        int itemH = item.GetHeight();

        // COMPROBACIÓN DE BORDES: Si la posición inicial + el tamaño del ítem superan los límites del maletín, no entra.
        if (startX + itemW > gridWidth || startY + itemH > gridHeight) return false;

        // DOBLE BUCLE: Recorre la sub-matriz de celdas que el objeto cubriría si se soltara en esa posición
        for (int x = 0; x < itemW; x++)
        {
            for (int y = 0; y < itemH; y++)
            {
                int currentX = startX + x; // Columna actual bajo análisis.
                int currentY = startY + y; // Fila actual bajo análisis.

                // --- Re-conversión inversa: de Coordenadas (X, Y) a Índice de Lista Plana ---
                int slotIndex = currentY * gridWidth + currentX;

                if (slotIndex >= slots.Count) return false; // Control de desborde de seguridad.

                Transform checkSlot = slots[slotIndex]; // Identifica el casillero real en esa coordenada.

                // COMPROBACIÓN DE OCUPACIÓN: Si el casillero ya está asociado a otro objeto en el diccionario, rebota el guardado.
                if (slotMap.ContainsKey(checkSlot) && slotMap[checkSlot] != null)
                {
                    return false; // Casillero ocupado.
                }
            }
        }
        return true; // Si superó todas las pruebas, la zona está completamente despejada.
    }

    // Gestiona la reubicación lógica y física de un ítem hacia su nuevo slot de destino
    public void PlaceItem(InventoryItem item, Transform targetSlot)
    {
        LiberarSlotsDeItem(item); // 1. Limpia sus marcas del lugar anterior para evitar duplicaciones lógicas.
        ForzarColocacionFísica(item, targetSlot); // 2. Ejecuta el acomodo visual y matemático en el nuevo slot.
    }

    // Modifica jerarquías, escalas, rotaciones y componentes físicos del objeto para calzarlo en la cuadrícula
    public void ForzarColocacionFísica(InventoryItem item, Transform targetSlot)
    {
        // Empaqueta el objeto volviéndolo hijo directo del casillero raíz seleccionado.
        item.transform.SetParent(targetSlot, false);
        item.transform.localPosition = new Vector3(0f, 0.5f, 0f); // Lo eleva ligeramente en el eje Y para que flote sobre el piso del inventario.
        item.transform.localRotation = Quaternion.identity;
        item.transform.localScale = Vector3.one;

        // --- AJUSTES GRÁFICOS MEDIANTE EL CONTENEDOR VISUAL ---
        if (item.contenedorVisual != null)
        {
            item.contenedorVisual.SetParent(item.transform, false);
            item.contenedorVisual.localPosition = Vector3.zero;

            // Aplica la escala de guardado (diseñada en el script del ítem) para que quepa bien visualmente en las celdas.
            item.contenedorVisual.localScale = item.escalaEnInventario;

            // Define su rotación visual final: lo acuesta (90 en X) y le suma 90 en Z si se encuentra en estado rotado.
            float anguloGiro = item.rotado ? 90f : 0f;
            item.contenedorVisual.localRotation = Quaternion.Euler(90f, 0f, anguloGiro);
        }
        else
        {
            // Si el ítem no usa contenedor separado, rota todo su cuerpo para reflejar el cambio.
            float anguloGiro = item.rotado ? 90f : 0f;
            item.transform.localRotation = Quaternion.Euler(90f, 0f, anguloGiro);
        }

        // --- AJUSTES FÍSICOS Y AUTOMÁTICOS DEL COLLIDER ---
        BoxCollider boxCol = item.GetComponent<BoxCollider>();
        if (boxCol != null)
        {
            boxCol.enabled = true; // Prende el collider para que el MouseManager lo detecte de nuevo mediante Raycast.
            boxCol.isTrigger = true; // Lo vuelve trigger para que no empuje físicamente al jugador ni cause glitches con el mapa.
            boxCol.center = Vector3.zero;
            boxCol.size = new Vector3(0.95f, 0.95f, 1.5f); // Redimensiona la caja de selección para que ocupe de manera óptima el casillero.
        }

        // --- ESTABILIZACIÓN DEL RIGIDBODY ---
        Rigidbody itemRb = item.GetComponent<Rigidbody>();
        if (itemRb != null)
        {
            itemRb.isKinematic = true; // Desactiva la gravedad y fuerzas físicas del motor sobre el ítem.
            itemRb.linearVelocity = Vector3.zero; // Cancela cualquier velocidad residual.
            itemRb.angularVelocity = Vector3.zero; // Cancela cualquier rotación física.
        }

        item.lastSlot = targetSlot; // El ítem recuerda este casillero exitoso como su posición de respaldo (por si falla futuros arrastres).

        // --- RESERVA LÓGICA EN EL DICCIONARIO ---
        // Obtiene la lista de todos los casilleros que este objeto va a tapar según su tamaño.
        List<Transform> requiredSlots = GetSlotsForItem(targetSlot, item.GetWidth(), item.GetHeight());
        foreach (Transform slot in requiredSlots)
        {
            slotMap[slot] = item; // Registra este ítem como dueño actual de esos casilleros en el diccionario.
        }
    }

    // Busca y borra cualquier rastro o registro de pertenencia de un ítem dentro del diccionario
    public void LiberarSlotsDeItem(InventoryItem item)
    {
        // Copia las llaves (Slots) en una lista temporal para poder modificarlas de forma segura dentro del bucle sin romper la colección.
        List<Transform> slotsAModificar = new List<Transform>(slotMap.Keys);
        foreach (Transform slot in slotsAModificar)
        {
            // Si el casillero le pertenecía a este objeto específico...
            if (slotMap[slot] == item)
            {
                slotMap[slot] = null; // Libera el casillero seteándolo de nuevo en vacío.
            }
        }
    }
    public void SpawnItemInGrid(string itemName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Items/{itemName}");
        if (prefab == null)
        {
            Debug.LogError($"[Inventario] No se encontró el prefab para '{itemName}' en Resources/Items/");
            return;
        }

        GameObject newItem = Instantiate(prefab, transform);
        InventoryItem invItem = newItem.GetComponent<InventoryItem>();
        ItemAnchorManager anchorManager = newItem.GetComponent<ItemAnchorManager>();

        if (invItem == null || anchorManager == null)
        {
            Debug.LogError($"[Inventario] El prefab '{itemName}' no tiene InventoryItem o ItemAnchorManager.");
            return;
        }

        // 🔹 Buscar el primer slot libre
        foreach (Transform slot in slots)
        {
            InventorySlot slotComp = slot.GetComponent<InventorySlot>();
            if (slotComp != null && !slotComp.ocupado)
            {
                // Colocar el ítem directamente sobre el slot
                newItem.transform.position = slot.position;
                newItem.transform.SetParent(slot);
                newItem.transform.localScale = invItem.escalaEnInventario;

                slotComp.ocupado = true;
                slotComp.itemActual = invItem;

                Debug.Log($"[Inventario] Ítem '{itemName}' colocado automáticamente en el slot '{slot.name}'.");
                return;
            }
        }

        Debug.LogWarning($"[Inventario] No hay espacio para '{itemName}' en el Grid.");
        Destroy(newItem);
    }




    // Método utilitario que devuelve una lista con todos los casilleros físicos involucrados a partir de un punto y un tamaño
    public List<Transform> GetSlotsForItem(Transform targetSlot, int width, int height)
    {
        List<Transform> foundSlots = new List<Transform>();
        int startIndex = slots.IndexOf(targetSlot);
        if (startIndex == -1) return foundSlots; // Si el slot de origen no existe, devuelve una lista vacía.

        // Mapea la posición lineal de origen a formato fila/columna (X, Y).
        int startX = startIndex % gridWidth;
        int startY = startIndex / gridWidth;

        // Recorre el bloque bidimensional sumando las dimensiones del objeto
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                int currentX = startX + x;
                int currentY = startY + y;

                // Calcula el índice lineal resultante de cada celda perteneciente al bloque.
                int slotIndex = currentY * gridWidth + currentX;

                // Si el índice calculado es válido dentro de la lista oficial, añade ese Transform a la lista de retorno.
                if (slotIndex < slots.Count)
                {
                    foundSlots.Add(slots[slotIndex]);
                }
            }
        }
        return foundSlots; // Devuelve la colección de celdas reservadas.
    }
}