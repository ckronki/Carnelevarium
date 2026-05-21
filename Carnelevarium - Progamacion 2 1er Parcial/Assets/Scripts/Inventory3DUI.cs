using System.Collections; // Librería requerida por C# para poder usar Corrutinas (IEnumerator), permitiendo movimientos suaves en el tiempo.
using TMPro; // Librería de TextMeshPro para controlar textos avanzados en la interfaz de usuario.
using UnityEngine; // Librería principal y nativa de Unity.
using UnityEngine.InputSystem; // Nueva librería de Input para detectar de forma moderna la tecla Tab.

public class Inventory3DUI : MonoBehaviour
{
    // --- VARIABLES EXPUESTAS EN EL INSPECTOR ---
    [Header("UI Feedback")]
    public TMP_Text inventoryMessage; // Texto en pantalla para mandarle avisos al jugador (Ej: "No hay espacio").

    [Header("Configuración del Inventario")]
    [Tooltip("Arrastra aquí el PREFAB 3D de tu inventario desde la carpeta de Assets")]
    public GameObject inventoryPrefab; // El archivo base (Prefab) del maletín o mochila 3D que aparecerá en el mundo.

    public Transform inventorySpotA; // Primera opción de ubicación (Marcador de posición en el suelo) donde puede spawnear el maletín.
    public Transform inventorySpotB; // Segunda opción de ubicación por si la primera está tapada por una pared o caja.

    [Header("Cámara")]
    public Camera playerCamera; // La cámara del jugador (la que vamos a desprender y mover).
    public Transform playerCameraSpot; // El punto de anclaje (hijo del Player) al que la cámara debe volver cuando cerremos el inventario.
    private CameraController camController; // Referencia interna al script de movimiento del mouse de la cámara.

    [Header("Player")]
    public GameObject playerSkin; // El modelo 3D del cuerpo del personaje (se oculta al abrir para que no tape la visual del maletín).
    public Player playerScript; // Referencia al script principal de movimiento y lógicas del personaje.

    [Header("Colisiones")]
    public LayerMask obstacleMask; // Capas de colisión que el sistema considerará como "obstáculos" (Paredes, enemigos, cajas).

    // --- VARIABLES INTERNAS PRIVADAS ---
    private GameObject activeInventoryInstance; // Guarda la copia real e instanciada del maletín que operará en el mapa.
    private bool isOpen = false; // Interruptor para saber si el inventario está abierto o cerrado en este momento.
    private bool isTransitioning = false; // Candado para evitar que el jugador spamee el Tab mientras la cámara está viajando.

    // Start se ejecuta al iniciar la escena
    void Start()
    {
        // Extrae el script del controlador de cámara desde el objeto de la cámara asignado.
        camController = playerCamera.GetComponent<CameraController>();

        // Asegura que la escala de tiempo del motor sea 1 (normal), evitando congelamientos por menús de pausa mal cerrados.
        Time.timeScale = 1f;

        // --- INSTANCIACIÓN AUTOMÁTICA AL INICIAR ---
        if (inventoryPrefab != null)
        {
            // Crea el maletín al iniciar la partida en una coordenada súper baja (-500 en Y), lejos de la vista, para tenerlo cargado en memoria.
            activeInventoryInstance = Instantiate(inventoryPrefab, new Vector3(0, -500f, 0), Quaternion.identity);

            // Lo desactiva inmediatamente. El inventario ya existe listo para usarse, pero está oculto esperando el llamado.
            activeInventoryInstance.SetActive(false);
        }
        else
        {
            // Error preventivo en rojo si me olvide de arrastrar el Prefab al componente en el Inspector.
            Debug.LogError("¡Falta asignar el Prefab del Inventario en el componente Inventory3DUI!");
        }
    }

    // Update busca pulsaciones de botones cada fotograma
    void Update()
    {
        // Si el jugador presiona la tecla TAB en este frame exacto...
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            ToggleInventory(); // Ejecuta el método para abrir o cerrar el inventario.
        }
    }

    // Función matemática que revisa si el maletín físico entra en un espacio determinado del suelo
    bool CanSpawnAt(Transform spot)
    {
        // Intenta obtener el Collider del punto de spawn asignado.
        Collider spotCollider = spot.GetComponent<Collider>();
        if (spotCollider == null) return true; // Si el punto no tiene collider para medir el área, asume de forma permisiva que está libre.

        // Extrae el tamaño (mitad de dimensiones) y el centro exacto de la caja límite (Bounds) del punto.
        Vector3 halfExtents = spotCollider.bounds.extents;
        Vector3 center = spotCollider.bounds.center;

        // Lanza una caja física invisible en esas coordenadas para detectar si choca con objetos en las capas configuradas como obstáculos.
        Collider[] hits = Physics.OverlapBox(center, halfExtents, spot.rotation, obstacleMask);

        // Analiza cada colisión encontrada por la caja invisible.
        foreach (Collider hit in hits)
        {
            // Si el choque NO es con el propio marcador y NO es con un hijo de este sistema, significa que es un obstáculo real (ej: una pared).
            if (hit.transform != spot && !hit.transform.IsChildOf(transform)) return false; // Retorna falso: el espacio está obstruido.
        }
        return true; // Si el bucle termina sin registrar problemas, retorna verdadero: el espacio está limpio.
    }

    // El cerebro del script: interviene los estados y gestiona el traspaso visual
    void ToggleInventory()
    {
        // CONTROL DE SEGURIDAD: Si la cámara se está moviendo o el maletín falló al crearse en el Start, detiene el proceso.
        if (isTransitioning || activeInventoryInstance == null) return;

        isOpen = !isOpen; // Invierte el estado del interruptor (si estaba cerrado pasa a abierto, y viceversa).

        // Detiene cualquier movimiento de cámara o corrutina que estuviera ejecutándose a medias para evitar bucles corruptos.
        StopAllCoroutines();

        if (isOpen) // --- LOGICA DE APERTURA ---
        {
            Transform chosenSpot = null; // Variable temporal para definir dónde acomodaremos el maletín.

            // Intenta primero con el Punto A. Si está libre, lo elige. Si está tapado, comprueba el Punto B.
            if (CanSpawnAt(inventorySpotA)) chosenSpot = inventorySpotA;
            else if (CanSpawnAt(inventorySpotB)) chosenSpot = inventorySpotB;

            // Si ambos puntos están bloqueados por obstáculos en el entorno...
            if (chosenSpot == null)
            {
                // Inicia la corrutina para imprimir un cartel flotante temporal en la UI del jugador.
                StartCoroutine(ShowMessage("No hay espacio para abrir el inventario", 2f));
                isOpen = false; // Devuelve el estado a cerrado.
                return; // Frena el código de apertura.
            }

            // Esconde avisos anteriores de la pantalla y oculta la ropa/cuerpo del jugador para que no obstruya la toma de la cámara.
            if (inventoryMessage != null) inventoryMessage.gameObject.SetActive(false);
            if (playerSkin != null) playerSkin.SetActive(false);

            // 1. Congela por completo las físicas y movimientos de control del personaje.
            if (playerScript != null)
            {
                playerScript.enabled = false; // Apaga el script del jugador para que no lea el teclado.
                if (playerScript.animator != null) playerScript.animator.SetBool("isWalking", false); // Apaga la animación de caminar.
                Rigidbody playerRb = playerScript.GetComponent<Rigidbody>();
                if (playerRb != null) playerRb.linearVelocity = Vector3.zero; // Detiene la inercia física de golpe para que no se deslice.
            }

            // 2. Apaga el script de la cámara para que el mouse del usuario deje de controlar la mirada del cuello.
            if (camController != null)
            {
                camController.enabled = false;
            }

            // 3. Teletransporta el maletín físico al punto libre seleccionado y lo enciende visualmente en el mundo.
            activeInventoryInstance.transform.position = chosenSpot.position;
            activeInventoryInstance.transform.rotation = chosenSpot.rotation;
            activeInventoryInstance.SetActive(true);

            // 4. Busca un objeto hijo dentro del Prefab del maletín que se llame obligatoriamente "CameraPoint".
            Transform camPoint = activeInventoryInstance.transform.Find("CameraPoint");
            if (camPoint != null)
            {
                // Inicia la animación de interpolación suave para mudar la cámara de la cabeza al punto del maletín.
                StartCoroutine(MoveCamera(camPoint.position, camPoint.rotation));
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }

            // Libera y muestra la flecha del cursor en la pantalla para poder arrastrar las celdas e ítems con comodidad.
            
        }
        else // --- LÓGICA DE CIERRE ---
        {
            activeInventoryInstance.SetActive(false); // Apaga y oculta el maletín tridimensional del mapa.

            if (playerSkin != null) playerSkin.SetActive(true); // Reaparece el modelo 3D del personaje.

            // 1. Inicia el viaje de regreso de la cámara hacia los ojos del personaje (playerCameraSpot).
            StartCoroutine(MoveCamera(playerCameraSpot.position, playerCameraSpot.rotation));

            // 2. TRUCO DE INMERSIÓN: Pasa la rotación final al script de la cámara para evitar que la mirada de la cabeza
            // pegue un latigazo o salto brusco al reactivarse los controles del mouse.
            if (camController != null)
            {
                camController.SincronizarAngulos(playerCameraSpot.rotation);
                camController.enabled = true; // Reactiva el control del mouse sobre la mirada.
            }

            // 3. Devuelve los mandos del teclado al script de movimiento del jugador.
            if (playerScript != null)
            {
                playerScript.enabled = true;

                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                
            }
                

            // Vuelve a capturar el cursor del mouse, ocultándolo en el centro para el juego en primera persona.
            

            if (inventoryMessage != null) inventoryMessage.gameObject.SetActive(false);
        }
    }

    // Corrutina utilitaria para mostrar un texto flotante en la interfaz y apagarlo tras unos segundos reales
    IEnumerator ShowMessage(string text, float duration)
    {
        if (inventoryMessage == null) yield break; // Seguridad: si no hay caja de texto asignada, cancela la acción.
        inventoryMessage.text = text; // Escribe el mensaje recibido en el componente de texto.
        inventoryMessage.gameObject.SetActive(true); // Prende el objeto de la interfaz.
        yield return new WaitForSecondsRealtime(duration); // Espera la cantidad de segundos fijada usando tiempo real (ignora pausas).
        inventoryMessage.gameObject.SetActive(false); // Apaga el objeto de la interfaz tras expirar el tiempo.
    }

    // Corrutina matemática (Lerp/Slerp) que desliza la cámara suavemente entre dos puntos del espacio
    IEnumerator MoveCamera(Vector3 targetPos, Quaternion targetRot)
    {
        isTransitioning = true; // Cierra el candado: la transición está activa.
        float duration = 0.4f; // El viaje de la cámara tomará exactamente 0.4 segundos.
        float elapsed = 0f; // Cronómetro interno que arranca en cero.

        GameManager.instance.player.CantMove();

        // Registra las coordenadas y rotaciones de salida exactas desde donde se encuentra la cámara al momento de pulsar el Tab.
        Vector3 startPos = playerCamera.transform.position;
        Quaternion startRot = playerCamera.transform.rotation;

        // Bucle temporal que correrá fotograma a fotograma hasta que el cronómetro alcance el tiempo de duración.
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Avanza el cronómetro usando el tiempo delta sin escala (para que funcione aun en pausa).
            float t = elapsed / duration; // Convierte el progreso en un porcentaje normalizado entre 0.0 y 1.0.

            // Interpola linealmente la posición en el espacio en base al porcentaje transcurrido.
            playerCamera.transform.position = Vector3.Lerp(startPos, targetPos, t);
            // Interpola de forma esférica (con suavidad matemática para giros) la rotación tridimensional.
            playerCamera.transform.rotation = Quaternion.Slerp(startRot, targetRot, t);

            yield return null; // Pausa la ejecución aquí y espera al siguiente fotograma del juego antes de continuar el bucle.
        }

        GameManager.instance.player.CanMove();
        
        if (!GameManager.instance.crowbarController.canAttack && GameManager.instance.player.hasCrowbar)
        {
            GameManager.instance.crowbarController.AttackUnlock();
        }
        else
        {
            GameManager.instance.crowbarController.AttackLock();
        }
            

        // CONTROL DE PRECISIÓN DE CIERRE: Fuerza la posición y rotación final exacta para corregir cualquier milésima de desfase numérico.
        playerCamera.transform.position = targetPos;
        playerCamera.transform.rotation = targetRot;
        isTransitioning = false; // Abre el candado: la cámara llegó a destino y el usuario puede interactuar de nuevo.
    }
}