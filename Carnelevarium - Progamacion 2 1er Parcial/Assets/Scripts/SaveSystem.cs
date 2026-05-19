using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class SaveSystem : MonoBehaviour
{
    [Header("Referencias")]
    public Player player;
    public Inventory inventory;

    [Header("Estado de la Sesión")]
    public List<string> objetosRecogidosEnEstaSesion = new List<string>();

    // Al despertar el script, limpiamos la carpeta de guardados para una sesión limpia
    void Awake()
    {
        LimpiarSesionPrevia();
    }

    private void LimpiarSesionPrevia()
    {
        string path = Application.persistentDataPath;
        string[] archivos = Directory.GetFiles(path, "save_*.json");

        foreach (string archivo in archivos)
        {
            try
            {
                File.Delete(archivo);
                Debug.Log("Sesión limpiada: Archivo eliminado -> " + archivo);
            }
            catch (System.Exception e)
            {
                Debug.LogError("No se pudo borrar el archivo: " + e.Message);
            }
        }
    }

    public void RegistrarObjetoDestruido(string id)
    {
        if (!objetosRecogidosEnEstaSesion.Contains(id))
        {
            objetosRecogidosEnEstaSesion.Add(id);
        }
    }

    public string GetPath(int slot)
    {
        return Application.persistentDataPath + "/save_" + slot + ".json";
    }

    // --- LÓGICA DE GUARDADO ---
    public void SaveGame(int slot)
    {
        SaveData data = new SaveData
        {
            x = player.transform.position.x,
            y = player.transform.position.y,
            z = player.transform.position.z,
            life = player.Life,
            items = new List<string>(inventory.GetItems()),
            collectedObjectIDs = new List<string>(objetosRecogidosEnEstaSesion)
        };

        // Guardar estado de los enemigos
        GuardarEstadoEnemigos(data);

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(GetPath(slot), json);
        Debug.Log("Juego guardado en el slot " + slot);
    }

    private void GuardarEstadoEnemigos(SaveData data)
    {
        data.savedEnemies = new List<EnemyPosData>();
        // Buscamos todos los que tengan EnemyIdentity, incluso desactivados
        EnemyIdentity[] todosLosEnemigos = Resources.FindObjectsOfTypeAll<EnemyIdentity>();

        foreach (var e in todosLosEnemigos)
        {
            if (e.gameObject.scene.name == null) continue; // Ignora prefabs de la carpeta Assets

            data.savedEnemies.Add(new EnemyPosData
            {
                id = e.enemyID,
                x = e.transform.position.x,
                y = e.transform.position.y,
                z = e.transform.position.z,
                isActive = e.gameObject.activeSelf
            });
        }
    }

    // --- LÓGICA DE CARGA (Segmento Actualizado) ---
    public void LoadGame(int slot)
    {
        string path = GetPath(slot);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            // 1. Posición y vida del jugador
            player.transform.position = new Vector3(data.x, data.y, data.z);
            player.Life = data.life;

            // 2. Sincronizar lista de objetos recogidos
            objetosRecogidosEnEstaSesion = new List<string>(data.collectedObjectIDs);

            // 3. Inventario 
            inventory.Clear();
            foreach (string item in data.items)
            {
                // Pasamos 'true' para avisar al inventario que es una carga y no sature la consola en tiempo real
                inventory.AddItem(item, true);
            }

            // 4. Actualizar Escena (Objetos e Items del mapa)
            ActualizarObjetosEnEscena(objetosRecogidosEnEstaSesion);

            // 5. Actualizar Enemigos
            CargarEstadoEnemigos(data.savedEnemies);

            Debug.Log("Cargado slot " + slot + ". Sincronización de mundo completa.");
        }
    }

    void ActualizarObjetosEnEscena(List<string> objetosRecogidos)
    {
        // Buscamos absolutamente todos los PickupItem en el proyecto (activos e inactivos)
        PickupItem[] objetosEnMapa = Resources.FindObjectsOfTypeAll<PickupItem>();

        foreach (PickupItem obj in objetosEnMapa)
        {
            // FILTRO CRÍTICO: Evita interactuar con los Prefabs de la carpeta Assets. Solo queremos objetos de la escena.
            if (obj.gameObject.scene.name == null) continue;

            // Si el ID de este objeto ya está en la lista de recolectados del archivo de guardado...
            if (objetosRecogidos.Contains(obj.objectID))
            {
                obj.gameObject.SetActive(false); // Se apaga dado que ya fue recogido con anterioridad
                Debug.Log($"[SaveSystem] Objeto '{obj.gameObject.name}' con ID [{obj.objectID}] DESACTIVADO (Ya cargado como recogido).");
            }
            else
            {
                obj.gameObject.SetActive(true); // Se activa/mantiene activo si no ha sido recolectado
                Debug.Log($"[SaveSystem] Objeto '{obj.gameObject.name}' con ID [{obj.objectID}] ACTIVADO (Disponible en el mapa).");
            }
        }
    }

    private void CargarEstadoEnemigos(List<EnemyPosData> listaGuardada)
    {
        if (listaGuardada == null) return;

        EnemyIdentity[] todosLosEnemigos = Resources.FindObjectsOfTypeAll<EnemyIdentity>();

        foreach (var e in todosLosEnemigos)
        {
            if (e.gameObject.scene.name == null) continue;

            EnemyPosData info = listaGuardada.Find(x => x.id == e.enemyID);
            if (info != null)
            {
                e.gameObject.SetActive(info.isActive);
                if (info.isActive)
                {
                    e.transform.position = new Vector3(info.x, info.y, info.z);
                }
            }
            else
            {
                // Si el enemigo no existe en el save (porque era una partida nueva o fue eliminado)
                e.gameObject.SetActive(false);
            }
        }
    }
}

