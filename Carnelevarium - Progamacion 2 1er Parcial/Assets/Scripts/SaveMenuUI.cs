using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.IO; // necesario para leer archivos
using TMPro; // <--- Añade esto arriba con los demás 'using'

public class SaveMenuUI : MonoBehaviour
{
    public GameObject menuPanel;
    public GameObject confirmPanel;
    public Button saveButtonPrefab;
    public Transform saveListContainer;

    public SaveSystem saveSystem;
    public CameraController cameraController; // referencia al script de cámara

    private List<Vector3> saves = new List<Vector3>();
    private int selectedIndex = -1;

    private void Update()
    {
        if (menuPanel.activeSelf && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            CloseMenu();
        }
    }

    public void OpenMenu()
    {
        menuPanel.SetActive(true);
        LoadAllSaves();
        RefreshList();

        // Bloquear cámara y cursor
        if (cameraController != null) cameraController.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0f;
    }

    public void CloseMenu()
    {
        menuPanel.SetActive(false);
        confirmPanel.SetActive(false);

        // Restaurar cámara y cursor
        if (cameraController != null) cameraController.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
    }

    public void AddNewSave()
    {
        // 1. Contamos cuántos archivos hay para crear el siguiente (ej: save_0, save_1...)
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "save_*.json");
        int nextSlot = files.Length;

        // 2. Guardamos el archivo físico
        saveSystem.SaveGame(nextSlot);

        // 3. ¡IMPORTANTE! Limpiamos y volvemos a leer TODO el disco
        // Esto asegura que 'saves.Count' aumente
        LoadAllSaves();

        // 4. Borramos los botones viejos y creamos los nuevos (incluyendo el recién creado)
        RefreshList();
    }

    private void LoadAllSaves()
    {
        saves.Clear();
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path, "save_*.json");

        // Ordenamos los archivos por nombre para que el 1 no vaya después del 10
        System.Array.Sort(files);

        foreach (string file in files)
        {
            string json = File.ReadAllText(file);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            // Guardamos la posición en nuestra lista interna para los botones
            saves.Add(new Vector3(data.x, data.y, data.z));
        }
    }
    private void RefreshList()
    {
        if (saveListContainer == null || saveButtonPrefab == null) return;

        foreach (Transform child in saveListContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < saves.Count; i++)
        {
            int index = i;
            Button btn = Instantiate(saveButtonPrefab, saveListContainer);

            // Buscamos el componente de TextMeshPro en lugar del Text antiguo
            TMP_Text textComponent = btn.GetComponentInChildren<TMP_Text>();

            if (textComponent != null)
            {
                textComponent.text = "Guardado " + (index + 1);
            }
            else
            {
                Debug.LogError("El prefab del botón no tiene un componente TextMeshPro.");
            }

            btn.onClick.AddListener(() => SelectSave(index));
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
        Debug.Log("Cargando guardado " + (index + 1));
        saveSystem.LoadGame(index);
    }


}
