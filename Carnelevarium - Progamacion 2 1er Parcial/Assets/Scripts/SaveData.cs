// --- CLASES DE DATOS (Asegúrate de que solo existan una vez en todo el proyecto) ---

using System.Collections.Generic;

[System.Serializable]
public class EnemyPosData
{
    public string id;
    public float x, y, z;
    public bool isActive;
}

[System.Serializable]
public class SaveData
{
    public float x, y, z;
    public int life;
    public List<string> items;
    public List<string> collectedObjectIDs;
    public List<EnemyPosData> savedEnemies; // Esta es la línea que necesitamos para los enemigos
}