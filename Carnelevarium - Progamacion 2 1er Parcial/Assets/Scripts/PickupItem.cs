using UnityEngine; // Librería base de Unity para Colliders, GameObjects y métodos de ciclo de vida.
using UnityEngine.InputSystem; // Nueva librería de Input para detectar de forma moderna la tecla E.

public class PickupItem : MonoBehaviour
{
    // --- VARIABLES EXPUESTAS EN EL INSPECTOR ---
    [Header("Identificación única")]
    public string objectID; // Código único para este objeto específico en el mapa (Ej: "Linterna_Pasillo_01"). Crucial para el guardado de partida.

    [Header("Prefabs y Referencias Visuales")]
    public GameObject itemPrefab; // El clon limpio (Prefab) de este objeto que tiene los scripts de grilla ('InventoryItem' e 'InventoryItemDrag').
    public GameObject flashlightObject; // El modelo 3D visual de la linterna que el jugador ve tirado en el suelo del escenario.

    // --- VARIABLES INTERNAS PRIVADAS ---
    private bool playerInRange = false; // Interruptor tipo sí/no que se activa cuando el jugador está lo suficientemente cerca del objeto.
    private InspectItem inspectSystem; // Referencia al sistema encargado de hacer flotar y rotar el ítem en la pantalla de inspección.

    // Start se ejecuta en el primer fotograma al iniciar la escena
    void Start()
    {
        // Busca automáticamente en la escena el componente 'InspectItem' y guarda su referencia.
        inspectSystem = FindObjectOfType<InspectItem>();
    }

    // Se activa automáticamente cuando otro objeto físico físico con Collider entra en el área "Trigger" de este objeto
    void OnTriggerEnter(Collider other)
    {
        // Si el objeto que entró tiene la etiqueta (Tag) "Player"...
        if (other.CompareTag("Player"))
        {
            playerInRange = true; // El jugador está en rango: habilitamos la interacción.
        }
    }

    // Se activa automáticamente cuando el objeto físico sale del área "Trigger"
    void OnTriggerExit(Collider other)
    {
        // Si el objeto que se alejó es el jugador...
        if (other.CompareTag("Player"))
        {
            playerInRange = false; // El jugador salió del rango: deshabilitamos la interacción.
        }
    }

    // Update busca pulsaciones de botones cada fotograma
    void Update()
    {
        // SI el jugador está cerca Y presiona la tecla 'E' en este frame exacto...
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            // Y SI el sistema de inspección NO está ocupado mostrando otro objeto actualmente...
            if (!inspectSystem.IsInspecting())
            {
                // Envia el objeto a la pantalla de inspección 3D.
                // Le pasa: 1. El prefab que irá al inventario, 2. El objeto actual del suelo, 3. Su ID de guardado.
                inspectSystem.SetItem(itemPrefab, this.gameObject, objectID);
            }
        }
    }

    // Este método público NO se ejecuta solo; lo invocará el script 'InspectItem' 
    // únicamente cuando el jugador acepte el objeto o presione ESC para guardarlo con éxito.
    public void GuardadoExitosoEnInventario()
    {
        // 1. Conexión con el sistema de persistencia/guardado.
        SaveSystem ss = FindObjectOfType<SaveSystem>();
        if (ss != null)
        {
            // Registra el ID de este objeto en la lista negra del juego para que, si el jugador cambia de mapa o recarga partida, este objeto no vuelva a aparecer (no respawnee).
            ss.RegistrarObjetoDestruido(objectID);
        }

        // 2. Ocultación del objeto en el escenario.
        // Si tienes asignado el modelo visual de la linterna del suelo...
        if (flashlightObject != null)
        {
            flashlightObject.SetActive(false); // Lo apaga por completo del mapa para que desaparezca de la vista inmediatamente.
        }

        // 3. Limpieza de memoria.
        // Destruye el contenedor principal (este script y su trigger) para liberar recursos y evitar que el jugador vuelva a presionar la 'E' en la nada.
        Destroy(this.gameObject);
    }
}