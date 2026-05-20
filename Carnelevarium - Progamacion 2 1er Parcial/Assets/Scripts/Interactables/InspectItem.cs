using UnityEngine; // Librería base de Unity para usar GameObjects, Transforms, Cámaras, etc.
using UnityEngine.InputSystem; // Nueva librería de Input de Unity para detectar teclado y mouse.

public class InspectItem : MonoBehaviour
{
    // --- VARIABLES EXPOSTAS EN EL INSPECTOR ---
    [Header("Zoom")]
    public float minScale = 0.5f;   // El límite de tamaño más pequeño al que se puede encoger el objeto con el zoom.
    public float maxScale = 2f;     // El límite de tamaño más grande al que puede crecer el objeto con el zoom.
    public float zoomSpeed = 0.1f;  // Qué tan rápido cambia el tamaño del objeto al mover la rueda del mouse.

    [Header("Cámara de inspección")]
    public Camera inspectCamera;    // Cámara secundaria dedicada exclusivamente a filmar el objeto flotante de inspección.

    // --- VARIABLES INTERNAS PRIVADAS ---
    private GameObject currentItem; // Guarda la referencia del clon tridimensional que está flotando frente a la cámara de inspección.
    private GameObject worldItem;   // Guarda la referencia del objeto real físico que estaba tirado en el suelo del mapa.
    public bool isInspecting = false; // Interruptor tipo sí/no para saber si el jugador está mirando un objeto en este momento.
    private GameObject itemPrefabRef; // Guarda el archivo original de la carpeta del proyecto para clonarlo después en el inventario.
    private PickupItem currentPickupRef; // Guarda el componente de recolección del objeto del suelo para avisarle si se guardó bien.
    private string currentID; // ID único del objeto (útil para sistemas de guardado de partidas o misiones).

    public Animator animator; // Controlador de animaciones (para activar la animación de las manos del jugador agarrando el ítem).

    // Update se ejecuta de forma automática una vez por cada fotograma del juego (Frame)
    void Update()
    {
        // CONTROL DE SEGURIDAD: Si no estamos inspeccionando o el objeto clonado no existe, frena el código de inmediato y no hace nada.
        if (!isInspecting || currentItem == null) return;

        // --- SISTEMA DE ROTACIÓN CON EL MOUSE ---
        // Detecta si el usuario mantiene presionado el clic izquierdo del mouse
        if (Mouse.current.leftButton.isPressed)
        {
            // Lee cuántos píxeles se movió el mouse en la pantalla en los ejes X (horizontal) e Y (vertical) desde el último frame.
            float rotX = Mouse.current.delta.x.ReadValue();
            float rotY = Mouse.current.delta.y.ReadValue();

            // Gira el objeto horizontalmente usando el movimiento horizontal del mouse. Se usa Space.World para evitar que los ejes se tuerzan.
            currentItem.transform.Rotate(Vector3.up, -rotX, Space.World);
            // Gira el objeto verticalmente usando el movimiento vertical del mouse.
            currentItem.transform.Rotate(Vector3.right, rotY, Space.World);
        }

        // --- SISTEMA DE ZOOM CON LA RUEDA (SCROLL) ---
        // Lee el movimiento de la rueda del mouse. Dará un número positivo hacia arriba y negativo hacia abajo.
        float scroll = Mouse.current.scroll.ReadValue().y;

        // Si el usuario está moviendo la rueda del mouse...
        if (scroll != 0)
        {
            // Calcula el multiplicador de tamaño. Si el scroll es positivo suma tamaño, si es negativo lo resta.
            float scaleChange = 1f + scroll * zoomSpeed;

            // Aplica el cambio multiplicando la escala actual del objeto por el nuevo modificador.
            currentItem.transform.localScale *= scaleChange;

            // CONTROL DE SEGURIDAD (CLAMP): Limita el valor en el eje X para que no supere el máximo configurado ni sea menor que el mínimo.
            float clampedScale = Mathf.Clamp(currentItem.transform.localScale.x, minScale, maxScale);
            // Sobreescribe la escala del objeto aplicando el límite en los tres ejes (X, Y, Z) de manera uniforme para mantener la proporción.
            currentItem.transform.localScale = Vector3.one * clampedScale;
        }

        // --- SISTEMA DE SALIDA ---
        // Si el jugador presiona la tecla Escape en este frame...
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            EndInspect(); // Llama a la función para cerrar la pantalla de inspección y guardar el ítem.
        }
    }

    // Esta función es llamada desde el script de interacción del suelo para iniciar todo el proceso
    public void SetItem(GameObject itemPrefab, GameObject worldReference, string id)
    {
        itemPrefabRef = itemPrefab; // Almacena el archivo original (Prefab) para recordar qué objeto estamos procesando.
        currentID = id; // Guarda el identificador del objeto.

        // Si el objeto del suelo existe, extrae su script 'PickupItem'; de lo contrario, guarda un valor vacío (null).
        currentPickupRef = worldReference != null ? worldReference.GetComponent<PickupItem>() : null;

        currentItem = Instantiate(itemPrefab); // Duplica en la escena una copia idéntica del prefab para que el jugador la examine.
        currentItem.layer = LayerMask.NameToLayer("Inspect"); // Cambia la capa del clon a "Inspect" para que solo la cámara de inspección pueda filmarlo.

        // Si el clon tiene el script de arrastrar del inventario, lo apaga para que no interfiera mientras flota en el menú de inspección.
        if (currentItem.TryGetComponent(out InventoryItemDrag drag)) drag.enabled = false;

        // Posiciona el clon flotante exactamente 2 unidades por delante de la cámara de inspección.
        currentItem.transform.position = inspectCamera.transform.position + inspectCamera.transform.forward * 2f;
        currentItem.transform.rotation = Quaternion.identity; // Resetea su rotación original a (0, 0, 0).
        currentItem.transform.localScale = Vector3.one; // Resetea su tamaño original a de fábrica (1, 1, 1).

        // Busca el colisionador del clon y lo apaga para que el mouse del inventario o físicas del juego no interactúen con él en el aire.
        Collider col = currentItem.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        worldItem = worldReference; // Guarda el objeto original del suelo para saber a cuál desaparecer si la recogida es exitosa.
        StartInspect(); // Enciende los parámetros del menú visual.
    }

    // Esta función limpia la pantalla, devuelve los controles y procesa el ingreso al inventario
    void EndInspect()
    {
        isInspecting = false; // Apaga el interruptor; el jugador ya no está inspeccionando.
        Time.timeScale = 1f; // Despausa el tiempo del juego para que el mundo se mueva normalmente de nuevo.

        // Busca el script de la cámara del jugador y vuelve a encenderlo para que pueda mover la cabeza.
        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl != null) camCtrl.enabled = true;

        // Busca el lector de controles del personaje y lo enciende para que pueda volver a caminar e interactuar.
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null) playerInput.enabled = true;

        // Bloquea el cursor del mouse en el centro de la pantalla (comportamiento estándar de juegos en primera persona).
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false; // Esconde la flecha del mouse de la pantalla.

        // Si el clon flotante de inspección todavía existe en el aire, lo destruye por completo para liberar memoria.
        if (currentItem != null) Destroy(currentItem);

        // --- PROCESAMIENTO E INTERCAMBIO AUTOMÁTICO DE PREFABS ---
        // Si el sistema de inventario está listo en la escena y recordamos qué objeto agarramos...
        if (InventorySystem.Instance != null && itemPrefabRef != null)
        {
            // Paso 1: Inicialmente asumimos que crearemos el mismo objeto que estaba en el suelo.
            GameObject prefabParaCrear = itemPrefabRef;

            // Paso 2: Convertimos el nombre del prefab a letras minúsculas para evitar errores tipográficos (ej: "Flashlight" -> "flashlight").
            string nombreLimpio = itemPrefabRef.name.ToLower();

            // Busca automáticamente en la carpeta especial 'Assets/Resources' un archivo que se llame igual pero con el sufijo "_inv" (ej: "tests_inv").
            GameObject prefabInvCorrecto = Resources.Load<GameObject>($"{nombreLimpio}_inv");

            // Si el sistema encuentra esa versión especial para el maletín...
            if (prefabInvCorrecto != null)
            {
                prefabParaCrear = prefabInvCorrecto; // Cambia el objetivo para instanciar la versión optimizada del inventario.
            }


            // Paso 3: Crea físicamente en el juego una instancia del prefab seleccionado listo para ser procesado.
            GameObject contenedorNuevo = Instantiate(prefabParaCrear);

            // Realiza una búsqueda profunda del script 'InventoryItem' (obligatorio para la cuadrícula) en la raíz, hijos o padres del objeto creado.
            InventoryItem invItem = contenedorNuevo.GetComponent<InventoryItem>();
            if (invItem == null) invItem = contenedorNuevo.GetComponentInChildren<InventoryItem>();
            if (invItem == null) invItem = contenedorNuevo.GetComponentInParent<InventoryItem>();

            // Paso 4: Si encontramos el script de datos del inventario...
            if (invItem != null)
            {
                // Le pide al InventorySystem que busque un espacio libre de forma automática para acomodar el ítem.
                if (InventorySystem.Instance.IntentarAñadirItemAuto(invItem))
                {
                    Debug.Log($"¡{contenedorNuevo.name} guardado con éxito en el inventario!");

                    // Si el objeto del suelo tenía el script de recogida activo, le avisa que la operación fue un éxito para destruirse de la escena real.
                    if (currentPickupRef != null) currentPickupRef.GuardadoExitosoEnInventario();
                }
                else
                {
                    // Si la cuadrícula está llena y el objeto no entra...
                    Debug.LogWarning("No queda espacio en el inventario.");
                    Destroy(contenedorNuevo); // Destruye el objeto clonado que no pudo entrar para no generar basura.
                    if (worldItem != null) worldItem.SetActive(true); // Vuelve a encender el objeto del suelo para que quede tirado donde estaba.
                }
            }
            else
            {
                // CONTROL DE ERRORES: Si el prefab instanciado no tenía el script 'InventoryItem', detiene el juego en la consola con un mensaje rojo.
                Debug.LogError($"[InspectItem] Error crítico: El objeto final '{contenedorNuevo.name}' no tiene el script 'InventoryItem'.");
                Destroy(contenedorNuevo); // Elimina el clon fallido.
                if (worldItem != null) worldItem.SetActive(true); // Devuelve el ítem original al suelo.
            }
        }

        Debug.Log("Modo inspección DESACTIVADO"); // Informa en la consola que el ciclo de inspección concluyó.
    }

    // Esta función inicializa los parámetros visuales, pausa el juego y libera el mouse
    void StartInspect()
    {
        isInspecting = true; // Enciende el interruptor principal de inspección.
        Time.timeScale = 0f; // Congela el tiempo del juego por completo (animaciones del mundo, enemigos y físicas se pausan).

        // Busca el control de la cámara del jugador y lo apaga para que la pantalla no se mueva con el mouse del juego.
        CameraController camCtrl = FindObjectOfType<CameraController>();
        if (camCtrl != null) camCtrl.enabled = false;

        // Busca el mapa de entradas de botones del jugador y lo apaga para que el personaje no camine ni dispare de fondo.
        PlayerInput playerInput = FindObjectOfType<PlayerInput>();
        if (playerInput != null) playerInput.enabled = false;

        // Desbloquea el puntero del mouse de las pantallas del juego para que pueda moverse libremente.
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true; // Hace que la flecha del mouse sea visible en pantalla para poder arrastrar y rotar.

        Debug.Log("Modo inspección ACTIVADO");

        // Activa el disparador (Trigger) en el componente Animator para reproducir la animación de levantar/acercar el objeto a la pantalla.
        animator.SetTrigger("TakingItem");
    }

    // Una función pública simplificada (tipo flecha) que cualquier otro script puede usar para saber desde afuera si el jugador está en el menú de inspección.
    public bool IsInspecting() => isInspecting;
}