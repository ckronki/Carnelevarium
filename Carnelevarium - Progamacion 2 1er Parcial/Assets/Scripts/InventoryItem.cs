using UnityEngine; // Librería base de Unity para acceder a MonoBehaviour, Transforms, Vectors y componentes del Inspector.

public class InventoryItem : MonoBehaviour
{
    // --- VARIABLES EXPOSTAS EN EL INSPECTOR (CONFIGURACIÓN) ---
    [Header("Configuración de Cuadrícula")]
    // 'width' representa el ancho del ítem medido en casilleros (columnas) dentro del inventario.
    [SerializeField] private int width = 1;

    // 'height' representa el alto del ítem medido en casilleros (filas) dentro del inventario.
    [SerializeField] private int height = 1;

    // --- VARIABLES PÚBLICAS DE ESTADO EN TIEMPO REAL ---
    [HideInInspector] // (Opcional) Esto oculta la variable del inspector para que no te confundas, ya que se controla por código.
    public bool rotado = false; // Interruptor tipo sí/no para saber si el jugador giró el objeto presionando la tecla 'R'.

    public Transform lastSlot; // Almacena la referencia del último casillero (Slot) válido donde estuvo asentado este objeto.

    [Header("Componentes Requeridos")]
    public Transform contenedorVisual; // Referencia al objeto hijo en la jerarquía que contiene los gráficos/mallas (Mesh) del ítem.

    [Header("Configuración de Tamaños Personalizados")]
    // Permite ajustar de forma independiente la escala tridimensional (X, Y, Z) del objeto cuando descansa dentro del maletín.
    [Tooltip("Escala del objeto cuando está guardado en las celdas del inventario")]
    public Vector3 escalaEnInventario = new Vector3(1f, 1f, 1f);

    // Permite ajustar la escala del objeto cuando el jugador lo levanta y lo mueve por la pantalla con el cursor.
    [Tooltip("Escala del objeto mientras lo tienes agarrado con el mouse")]
    public Vector3 escalaAlArrastrar = new Vector3(1f, 1f, 1f);

    // --- MÉTODOS Y FUNCIONES DE RETORNO (PROPIEDADES DE TAMAÑO) ---

    // Función tipo flecha (simplificada) para obtener el ancho real del objeto en la cuadrícula.
    // TRUCO MATEMÁTICO: Si el objeto está rotado (true), su ancho actual pasa a ser su 'height' original. Si no, devuelve su 'width' de fábrica.
    public int GetWidth() => rotado ? height : width;

    // Función tipo flecha para obtener el alto real del objeto en la cuadrícula.
    // TRUCO MATEMÁTICO: Si el objeto está rotado (true), su altura actual pasa a ser su 'width' original. Si no, devuelve su 'height' de fábrica.
    public int GetHeight() => rotado ? width : height;

    // ---MÉTODOS DE ACCIÓN ---

    // Este método es llamado por el MouseManager cada vez que el jugador presiona la tecla 'R' con el objeto seleccionado.
    public void Rotate()
    {
        rotado = !rotado; // Invierte el valor actual del booleano (si era falso lo vuelve verdadero, si era verdadero lo vuelve falso).
    }
}