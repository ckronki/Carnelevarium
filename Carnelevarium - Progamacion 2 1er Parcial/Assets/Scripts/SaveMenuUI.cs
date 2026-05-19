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

    private string savePathFolder;

    // ====================================================================
    // MODIFICACIÓN CRÍTICA: LIMPIEZA AUTOMÁTICA AL INICIAR LA ESCENA
    // ====================================================================
    void Awake()
    {
        savePathFolder = Application.persistentDataPath;

        // EJECUCIÓN INMEDIATA: Cada vez que se carga la escena o das Play,
        // el script escanea la carpeta y elimina todos los archivos de guardado.
        EliminarArchivosDeGuardadoDelDisco();
    }

    private void Update()
    {
        // 1. CERRAR CON ESCAPE
        if (menuPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }

        // 2. TEMPORIZADOR DEL AVISO VISUAL (5 Segundos)
        if (tiempoParaOcultarAviso > 0f)
        {
            if (Time.unscaledTime >= tiempoParaOcultarAviso)
            {
                avisoLimiteCanvas.SetActive(false);
                tiempoParaOcultarAviso = -1f;
            }
        }
    }

    // Método interno encargado de realizar la purga en tu PC
    private void EliminarArchivosDeGuardadoDelDisco()
    {
        try
        {
            // Busca cualquier archivo que coincida con tu formato de guardados JSON
            string[] archivosJson = Directory.GetFiles(savePathFolder, "save_*.json");

            if (archivosJson.Length > 0)
            {
                foreach (string archivo in archivosJson)
                {
                    File.Delete(archivo); // Borrado físico
                    Debug.Log($"<color=red>[Auto-Clean]</color> Archivo eliminado al cargar escena: {Path.GetFileName(archivo)}");
                }
                Debug.Log("<color=red>[Auto-Clean]</color> Todos los guardados anteriores han sido eliminados con éxito.");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[Auto-Clean] Error al intentar limpiar la carpeta de guardados: {e.Message}");
        }
    }

    public bool HaAlcanzadoElLimite()
    {
        string[] files = Directory.GetFiles(savePathFolder, "save_*.json");
        return files.Length >= MAX_SAVES;
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        LoadAllSaves(); // Busca los archivos JSON en el disco
        RefreshList();  // Dibuja los botones en la interfaz

        if (menuLimitWarningText != null)
        {
            menuLimitWarningText.gameObject.SetActive(HaAlcanzadoElLimite());
            menuLimitWarningText.text = "Slots llenos (5/5). Solo puedes cargar partidas existentes.";
        }

        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f; // Pausa el juego
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        confirmPanel.SetActive(false);

        if (avisoLimiteCanvas != null) avisoLimiteCanvas.SetActive(false);
        tiempoParaOcultarAviso = -1f;

        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f; // Reanuda el juego
    }

    public void AddNewSave()
    {
        if (HaAlcanzadoElLimite())
        {
            Debug.LogWarning("[SAVE MENU] No se puede crear un nuevo archivo. Límite de 5 alcanzado.");
            if (avisoLimiteCanvas != null)
            {
                avisoLimiteCanvas.SetActive(true);
                tiempoParaOcultarAviso = Time.unscaledTime + 5f;
            }
            return;
        }

        int nextSlot = ObtenerSiguienteIndexLibre();

        if (saveSystem != null)
        {
            saveSystem.SaveGame(nextSlot);
        }

        LoadAllSaves();
        RefreshList();

        if (menuLimitWarningText != null)
        {
            menuLimitWarningText.gameObject.SetActive(HaAlcanzadoElLimite());
        }
    }

    private int ObtenerSiguienteIndexLibre()
    {
        for (int i = 0; i < MAX_SAVES; i++)
        {
            string archivoVerificar = Path.Combine(savePathFolder, $"save_{i}.json");
            if (!File.Exists(archivoVerificar))
            {
                return i;
            }
        }
        return 0;
    }

    private void LoadAllSaves()
    {
        saves.Clear();
        string[] files = Directory.GetFiles(savePathFolder, "save_*.json");

        System.Array.Sort(files);

        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                SaveSystem.SaveData data = JsonUtility.FromJson<SaveSystem.SaveData>(json);
                saves.Add(new Vector3(data.x, data.y, data.z));
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[SaveMenuUI] Error al leer JSON: {file}. Detalle: {e.Message}");
            }
        }
    }

    private void RefreshList()
    {
        if (saveListContainer == null || saveButtonPrefab == null) return;

        foreach (Transform child in saveListContainer)
        {
            Destroy(child.gameObject);
        }

        string[] files = Directory.GetFiles(savePathFolder, "save_*.json");
        System.Array.Sort(files);

        for (int i = 0; i < files.Length; i++)
        {
            int index = i;

            string nombreArchivo = Path.GetFileNameWithoutExtension(files[i]);
            string numeroStr = nombreArchivo.Replace("save_", "");
            int.TryParse(numeroStr, out int numeroRealSlot);

            Button btn = Instantiate(saveButtonPrefab, saveListContainer);
            TMP_Text textComponent = btn.GetComponentInChildren<TMP_Text>();

            if (textComponent != null)
            {
                textComponent.text = "Guardado " + (numeroRealSlot + 1);
            }

            btn.onClick.AddListener(() => SelectSave(numeroRealSlot));
        }
    }

    private void SelectSave(int index)
    {
        selectedIndex = index;
        confirmPanel.SetActive(true);
    }

    public void ConfirmLoad()
    {
        if (selectedIndex >= 0)
        {
            LoadSave(selectedIndex);
        }
    }

    private void LoadSave(int index)
    {
        confirmPanel.SetActive(false);
        Debug.Log("Cargando el archivo en el slot index: " + index);

        if (saveSystem != null)
        {
            saveSystem.LoadGame(index);
        }

        CloseMenu();
    }
}