using UnityEngine;

[CreateAssetMenu(fileName = "NuevoItemGrid", menuName = "Inventario/Item Grid")]
public class ItemData : ScriptableObject
{
    public string id;
    public string nombre;
    public Sprite icono; // opcional, para UI
    public GameObject prefab3D; // el modelo 3D que se instancia en el inventario

    [Header("Dimensiones en la Cuadrícula")]
    public int ancho = 1;
    public int alto = 1;

    public int GetAncho(bool rotado) => rotado ? alto : ancho;
    public int GetAlto(bool rotado) => rotado ? ancho : alto;
}
