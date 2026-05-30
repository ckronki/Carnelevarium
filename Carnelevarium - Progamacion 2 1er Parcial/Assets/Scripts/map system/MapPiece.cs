using UnityEngine;

[CreateAssetMenu(fileName = "NuevaParteMapa", menuName = "Mapa/Parte")]
public class MapPiece : ScriptableObject
{
    public string id;          // Ej: "Norte", "Centro", "Sur"
    public string nombre;      // Nombre descriptivo
    public Sprite zonaSprite;  // Imagen de la zona en el mapa
}
