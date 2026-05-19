using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO;
using TMPro;

public class SaveMenuUI : MonoBehaviour
{
    [Header("Paneles de la Interfaz")]
    public GameObject menuPanel;          // Panel principal del menú de guardado
    public GameObject confirmPanel;       // Panel de confirmación al seleccionar un archivo

    [Header("Componentes de la Lista de Guardados")]
    public Button saveButtonPrefab;       // Prefab del botón que representará cada archivo
    public Transform saveListContainer;   // Contenedor (Grid/Vertical Layout) donde se meten los botones

    [Header("UI de Advertencias")]
    [Tooltip("Opcional: Texto estático que avisa de forma permanente dentro del menú si se llegó al límite.")]
    public TMP_Text menuLimitWarningText;

    [Tooltip("CRÍTICO: El objeto del Canvas que aparecerá durante 5 segundos al intentar sobrepasar el límite.")]
    public GameObject avisoLimiteCanvas;

    [Header("Referencias del Sistema")]
    public SaveSystem saveSystem;         // Referencia a tu script de guardado lógico
    public CameraController cameraController; // Referencia para bloquear/desbloquear la cámara del jugador

    // Variables internas de control
    private List<Vector3> saves = new List<Vector3>(); // Lista local con las posiciones cargadas
    private int selectedIndex = -1;                    // Índice del archivo seleccionado actualmente
    private const int MAX_SAVES = 5;                   // Límite estricto de archivos permitidos
    private float tiempoParaOcultarAviso = -1f;        // Cronómetro para el texto de advertencia de 5 segundos

    private void Update()
    {
        // 1. CERRAR CON ESCAPE: Si el menú está abierto y se pulsa Escape, se cierra
        if (menuPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }

        // 2. TEMPORIZADOR DEL AVISO VISUAL: Si el cronómetro está activo (es mayor que 0)
        if (tiempoParaOcultarAviso > 0f)
        {
            // 'Time.unscaledTime' mide el tiempo real del reloj físico. 
            // Ignora si el juego está pausado con Time.timeScale = 0
            if (Time.unscaledTime >= tiempoParaOcultarAviso)
            {
                avisoLimiteCanvas.SetActive(false); // Apagamos el objeto en el Canvas
                tiempoParaOcultarAviso = -1f;       // Reseteamos el cronómetro
                Debug.Log("[SAVE MENU] Tiempo cumplido. Texto de advertencia desactivado automáticamente.");
            }
        }
    }

    /// <summary>
    /// Revisa de forma dinámica en el disco cuántos archivos "save_*.json" existen.
    /// Devuelve TRUE si ya hay 5 o más.
    /// </summary>
    public bool HaAlcanzadoElLimite()
    {
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "save_*.json");
        return files.Length >= MAX_SAVES;
    }

    /// <summary>
    /// Abre el menú, pausa el juego, libera el cursor y refresca la lista de archivos.
    /// </summary>
    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        LoadAllSaves(); // Busca los archivos en el disco para que puedas verlos y cargarlos
        RefreshList();  // Dibuja los botones en la interfaz

        // Si tienes un texto fijo de advertencia en el menú, actualiza su visibilidad
        if (menuLimitWarningText != null)
        {
            menuLimitWarningText.gameObject.SetActive(HaAlcanzadoElLimite());
            menuLimitWarningText.text = "Slots llenos (5/5). Solo puedes cargar partidas existentes.";
        }

        // Pausa del juego y liberación del mouse
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // El tiempo del juego se detiene aquí
    }

    /// <summary>
    /// Cierra las ventanas del menú, limpia alertas temporales y reanuda el juego.
    /// </summary>
    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // Desactivamos el aviso de 5 segundos por si acaso seguía activo al cerrar el menú
        if (avisoLimiteCanvas != null) avisoLimiteCanvas.SetActive(false);
        tiempoParaOcultarAviso = -1f; // Reseteamos el cronómetro

        // Reactivación del control del jugador y el tiempo
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; // El tiempo del juego vuelve a la normalidad
    }

    /// <summary>
    /// Intenta crear un nuevo archivo de guardado. Bloquea la acción si ya hay 5 archivos.
    /// </summary>
    public void AddNewSave()
    {
        // COMPROBACIÓN DEL LÍMITE:
        if (HaAlcanzadoElLimite())
        {
            Debug.LogWarning("[SAVE MENU] No se puede crear un nuevo archivo. Límite de 5 alcanzado.");

            // Si el objeto fue asignado en el inspector, lo activamos e iniciamos el conteo
            if (avisoLimiteCanvas != null)
            {
                avisoLimiteCanvas.SetActive(true); // Se muestra inmediatamente en pantalla

                // Calculamos el segundo exacto en el futuro en el que se debe apagar (Tiempo actual + 5 segundos)
                tiempoParaOcultarAviso = Time.unscaledTime + 5f;
            }
            else
            {
                Debug.LogError("[SAVE MENU] Error: No has arrastrado el 'avisoLimiteCanvas' en el Inspector.");
            }

            return; // Cortamos la ejecución para que NO guarde nada nuevo, pero el menú se queda abierto
        }

        // PROCESO DE GUARDADO NORMAL (Solo se ejecuta si hay menos de 5 archivos):
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "save_*.json");
        int nextSlot = files.Length;

        saveSystem.SaveGame(nextSlot); // Ordena al SaveSystem escribir el archivo .json
        LoadAllSaves();                // Recarga la lista interna de posiciones
        RefreshList();                 // Re-dibuja los botones en la interfaz

        // Actualiza el texto fijo del menú si este último guardado completó los 5 slots
        if (menuLimitWarningText != null)
        {
            menuLimitWarningText.gameObject.SetActive(HaAlcanzadoElLimite());
        }
    }

    /// <summary>
    /// Lee los archivos JSON del disco y extrae los datos de posición a la lista local.
    /// </summary>
    private void LoadAllSaves()
    {
        saves.Clear();
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "save_*.json");

        System.Array.Sort(files); // Los ordena alfabéticamente para mantener el orden (0, 1, 2...)

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            saves.Add(new Vector3(data.x, data.y, data.z)); // Guardamos la posición en la lista interna
        }
    }

    /// <summary>
    /// Destruye los botones viejos y genera nuevos basados en los archivos detectados en el disco.
    /// </summary>
    private void RefreshList()
    {
        if (saveListContainer == null || saveButtonPrefab == null) return;

        // Limpieza de botones antiguos en el contenedor UI
        foreach (Transform child in saveListContainer)
        {
            Destroy(child.gameObject);
        }

        // Creación dinámica de botones por cada guardado encontrado
        for (int i = 0; i < saves.Count; i++)
        {
            int index = i; // Copia local del índice para evitar errores en el listener del botón
            Button btn = Instantiate(saveButtonPrefab, saveListContainer);
            TMP_Text textComponent = btn.GetComponentInChildren<TMP_Text>();

            if (textComponent != null)
            {
                textComponent.text = "Guardado " + (index + 1); // Nombra el botón: "Guardado 1", "Guardado 2", etc.
            }
            else
            {
                Debug.LogError("El prefab del botón no tiene un componente TextMeshPro adjunto.");
            }

            // Al hacer click, este botón seleccionará su propio índice de guardado para permitirte cargarlo
            btn.onClick.AddListener(() => SelectSave(index));
        }
    }

    /// <summary>
    /// Registra qué índice se clickeó y despliega el panel flotante de confirmación.
    /// </summary>
    private void SelectSave(int index)
    {
        selectedIndex = index;
        confirmPanel.SetActive(true); // Abre la ventanita de "¿Estás seguro de cargar?"
    }

    /// <summary>
    /// Se ejecuta desde el botón 'Aceptar' del panel de confirmación.
    /// </summary>
    public void ConfirmLoad()
    {
        if (selectedIndex >= 0)
        {
            LoadSave(selectedIndex);
        }
    }

    /// <summary>
    /// Ejecuta la carga final del slot seleccionado y cierra la interfaz.
    /// </summary>
    private void LoadSave(int index)
    {
        confirmPanel.SetActive(false);
        Debug.Log("Cargando el archivo en el slot index: " + index);
        saveSystem.LoadGame(index); // Envía la orden de carga de datos al Player
        CloseMenu(); // Cierra el menú y reanuda el juego automáticamente al cargar
    }
}