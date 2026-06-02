using UnityEngine; // Librería base de Unity para componentes, físicas y GameObjects.
using UnityEngine.InputSystem; // Nueva librería de Input para detectar pulsaciones de teclado (tecla E).
using UnityEngine.Rendering; // Librería para configuraciones de renderizado (no se usa activamente en este fragmento).

public class InteractionController : MonoBehaviour
{
    // --- VARIABLES EXPOSTAS EN EL INSPECTOR ---
    [SerializeField] Camera _playerCamera; // La cámara principal del jugador en primera persona (desde donde nace el rayo).
    public CameraController _cameraController; // Referencia al script que controla el movimiento de la cabeza/cámara del jugador.
    [SerializeField] float _interactionDistance; // Distancia máxima en metros a la que el jugador puede interactuar con los objetos.
    [SerializeField] float _interactionDistanceBackup; // Variable de seguridad para recordar la distancia original cuando el sistema se bloquea.
    [SerializeField] GameObject _interactionCrosshair; // El elemento de la interfaz de usuario (UI) que se enciende cuando miras un objeto interactivo.

    // --- VARIABLES INTERNAS PRIVADAS ---
    IInteractable _currentTargetedInteractable; // Guarda la interfaz del objeto al que estamos mirando actualmente (si es que se puede interactuar con él).

    public void Start()
    {
        _interactionDistanceBackup = _interactionDistance;
    }

    // Update se ejecuta automáticamente una vez por cada fotograma del juego
    public void Update()
    {
        UpdateCurrentInteractable(); // Paso 1: Busca si hay un objeto interactivo frente al jugador.
        UpdateInteractionCrosshair(); // Paso 2: Enciende o apaga la retícula en la pantalla según el paso 1.
        CheckForInteractionInput(); // Paso 3: Revisa si el jugador presiona la tecla de interactuar.



    }

    // Lanza un rayo físico desde el centro de la cámara para detectar objetos interactivos
    public void UpdateCurrentInteractable()
    {
        // Crea un rayo matemático que nace en la cámara y apunta hacia el centro exacto de la pantalla (0.5f en X, 0.5f en Y del Viewport).
        var ray = _playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        // Realiza una prueba física en el mundo (Raycast). Si el rayo choca con un colisionador dentro de la distancia máxima, guarda los datos en 'hit'.
        Physics.Raycast(ray, out var hit, _interactionDistance);

        // Operador '?.': Si chocamos con algo (hit.collider no es nulo), intenta buscar el script que tenga la interfaz 'IInteractable'. 
        // Si no chocamos con nada o el objeto no es interactivo, guarda un valor vacío (null).
        _currentTargetedInteractable = hit.collider?.GetComponent<IInteractable>();
    }

    // Controla la presencia visual de la retícula de interacción en la interfaz
    public void UpdateInteractionCrosshair()
    {
        // Si no estamos apuntando a ningún objeto interactivo válido...
        if (_currentTargetedInteractable == null)
        {
            _interactionCrosshair.SetActive(false); // Apaga el gráfico de la retícula en la pantalla.
            return; // Termina la función aquí para no ejecutar el código de abajo.
        }
        else // Si detectamos un objeto con la interfaz 'IInteractable'...
        {
            _interactionCrosshair.SetActive(true); // Enciende la retícula en la pantalla para indicarle al jugador que puede interactuar.
        }
    }

    // Escucha el teclado para activar la interacción
    public void CheckForInteractionInput()
    {

        // Si el jugador presionó la tecla 'E' en este fotograma EXACTO y además hay un objeto válido en la mira...
        if (Keyboard.current.eKey.wasPressedThisFrame && _currentTargetedInteractable != null)
        {
            // Ejecuta el método universal 'Interact()' que tiene el objeto guardado (cada objeto responderá a su manera: abrir puerta, prender linterna, etc.).
            _currentTargetedInteractable.Interact();
        }
    }

    // Bloquea por completo la capacidad de interactuar y mover la cámara (útil para menús, cinemáticas o el modo inspección)
    public void LockInteraction()
    {
        // Imprime en la consola el estado de bloqueo actual de la cámara antes de cambiarlo.
        Debug.Log("Cámara bloqueada: " + _cameraController.isCameraLocked);

        // Llama al método del CameraController para congelar el movimiento del mouse del personaje.
        _cameraController.LockCamera();

        // Reduce la distancia de interacción a cero. Al hacer esto, el Raycast nunca llegará a tocar nada y no se podrá interactuar con nada.
        _interactionDistance = 0;
    }

    // Desbloquea el sistema devolviendo el control al jugador
    public void UnlockInteraction()
    {
        // Imprime en la consola el estado de bloqueo de la cámara.
        Debug.Log("Cámara bloqueada: " + _cameraController.isCameraLocked);

        // Llama al método del CameraController para volver a liberar el movimiento del mouse del personaje.
        _cameraController.UnlockCamera();

        // Restaura la distancia de interacción original que habíamos respaldado en el método LockInteraction.
        _interactionDistance = _interactionDistanceBackup;
    }
}