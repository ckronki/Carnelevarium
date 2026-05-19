using UnityEngine; // Librería base de Unity para operaciones esenciales con GameObjects, Transforms e Instanciaciones.

public class ItemTransformer : MonoBehaviour
{
    // --- VARIABLES EXPUESTAS EN EL INSPECTOR ---
    [Header("Asigna aquí el Prefab flashLight_inv")]
    [Tooltip("Arrastra aquí el Prefab moderno y corregido que sí tiene el Contenedor Visual configurado")]
    public GameObject prefabInventarioCorrecto; // El archivo base (Prefab) del ítem compatible con el inventario 3D.

    // Awake se ejecuta inmediatamente cuando el objeto se crea en memoria, incluso antes que el Start
    void Awake()
    {
        // --- FILTRO DE SEGURIDAD Y DETECCIÓN ---
        // Comprueba si este objeto posee el script 'InventoryItem' pero carece de un 'contenedorVisual' asignado.
        // Si se cumple, significa que es una versión vieja, incompleta o diseñada solo para decoración del mapa.
        if (GetComponent<InventoryItem>() != null && GetComponent<InventoryItem>().contenedorVisual == null)
        {
            // Imprime un aviso amarillo preventivo en la consola de Unity para avisarte de la conversión automática.
            Debug.LogWarning("[SISTEMA] Se detectó la creación de la linterna vieja. Reemplazando por la versión de inventario...");

            // ==========================================
            // 1. ACCIÓN: INSTANCIACIÓN DE REEMPLAZO
            // ==========================================
            // Clona el prefab correcto exactamente en las mismas coordenadas tridimensionales (posición y rotación) del objeto viejo.
            GameObject clonCorrecto = Instantiate(prefabInventarioCorrecto, transform.position, transform.rotation);

            // Le limpia el texto "(Clone)" del nombre para evitar problemas si otros sistemas buscan el objeto por su string de identidad.
            clonCorrecto.name = prefabInventarioCorrecto.name;

            // ==========================================
            // 2. ACCIÓN: TRASPASO DE JERARQUÍA Y REGISTRO
            // ==========================================
            // Si el objeto viejo ya estaba asignado a un contenedor padre (por ejemplo, si nació directamente adentro de un Slot del inventario)...
            if (transform.parent != null)
            {
                // Muda al clon correcto para que se vuelva hijo de ese mismo padre, manteniendo las coordenadas relativas en falso.
                clonCorrecto.transform.SetParent(transform.parent, false);

                // Extrae el componente 'InventoryItem' de la linterna nueva.
                InventoryItem nuevoItemScript = clonCorrecto.GetComponent<InventoryItem>();

                // Le ordena al InventorySystem central que registre, empaquete y fuerce la colocación del nuevo ítem en ese slot.
                // Esto recalcula el diccionario, limpia las celdas y le da estabilidad física y de colisiones al clon.
                InventorySystem.Instance.PlaceItem(nuevoItemScript, transform.parent);
            }

            // ==========================================
            // 3. ACCIÓN: ELIMINACIÓN DEL OBJETO OBSOLETO
            // ==========================================
            // Remueve por completo el objeto viejo defectuoso de la escena actual en este fotograma.
            // Esto libera memoria RAM, evita colisiones duplicadas, glitches visuales y errores de escala en el maletín.
            Destroy(gameObject);
        }
    }
}