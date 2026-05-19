using UnityEngine; // Librería base de Unity necesaria para crear contenedores de datos como ScriptableObjects.

// Atributo que le indica a Unity añadir esta clase al menú del botón derecho del ratón en la ventana 'Project' (Assets).
// 'fileName' define el nombre por defecto que tendrá el archivo creado.
// 'menuName' estructura las pestañas y submenús del menú desplegable para organizarlo limpiamente.
[CreateAssetMenu(fileName = "NuevoItemGrid", menuName = "Inventario/Item Grid")]
public class ItemData : ScriptableObject
{
    // --- DATOS DE IDENTIFICACIÓN GENERAL ---
    public string id; // Código o clave única alfanumérica para el sistema (Ej: "W_9mm_Pistol", "M_Medkit_01").
    public string nombre; // El nombre legible que verá el jugador en la interfaz del juego (Ej: "Pistola de 9mm").
    public Sprite icono; // Imagen en 2D (textura) opcional, útil si diseñas pantallas secundarias de compra, venta o HUD flotante.
    public GameObject prefab3D; // El archivo base (Prefab) del modelo geométrico 3D real que se va a clonar e instanciar dentro del maletín.

    // --- VARIABLES DE TAMAÑO BASE ---
    [Header("Dimensiones en la Cuadrícula")]
    public int ancho = 1; // Cuántos casilleros (columnas) ocupa este objeto de forma nativa en horizontal.
    public int alto = 1; // Cuántos casilleros (filas) ocupa este objeto de forma nativa en vertical.

    // --- MÉTODOS MATEMÁTICOS DE RETORNO (PROPIEDADES DE TAMAÑO) ---

    // Función simplificada (tipo flecha) para calcular el ancho final de este tipo de objeto según su estado de giro.
    // TRUCO: Si el parámetro 'rotado' es verdadero (true), intercambia su ancho por su altura original. Si es falso, devuelve su ancho de fábrica.
    public int GetAncho(bool rotado) => rotado ? alto : ancho;

    // Función simplificada (tipo flecha) para calcular el alto final de este tipo de objeto según su estado de giro.
    // TRUCO: Si el parámetro 'rotado' es verdadero (true), intercambia su altura por su ancho original. Si es falso, devuelve su altura de fábrica.
    public int GetAlto(bool rotado) => rotado ? ancho : alto;
}