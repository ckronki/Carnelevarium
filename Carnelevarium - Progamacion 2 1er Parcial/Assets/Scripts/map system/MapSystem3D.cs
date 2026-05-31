using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapSystem3D : MonoBehaviour
{
    public static MapSystem3D Instance { get; private set; }

    public Transform player;
    public GameObject mapa3D; // el objeto físico del mapa
    public Dictionary<string, bool> piezasObtenidas = new Dictionary<string, bool>();

    private bool isOpen = false;

    void Awake()
    {
        Instance = this;
        piezasObtenidas["Norte"] = false;
        piezasObtenidas["Centro"] = false;
        piezasObtenidas["Sur"] = false;
    }

    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMap();
        }

        if (isOpen)
        {
            ActualizarMapa();
        }
    }

    public void AddPiece(string piezaID)
    {
        piezasObtenidas[piezaID] = true;
    }

    void ToggleMap()
    {
        isOpen = !isOpen;
        mapa3D.SetActive(isOpen);
    }

    void ActualizarMapa()
    {
        foreach (var pieza in piezasObtenidas)
        {
            if (pieza.Value && JugadorDentroDeZona(pieza.Key))
            {
                // Titilar en rojo la zona actual
                Material mat = mapa3D.transform.Find(pieza.Key).GetComponent<Renderer>().material;
                mat.color = Color.Lerp(Color.white, Color.red, Mathf.PingPong(Time.time, 1));
            }
        }
    }

    bool JugadorDentroDeZona(string zonaID)
    {
        if (zonaID == "Norte" && player.position.z > 50) return true;
        if (zonaID == "Centro" && player.position.z <= 50 && player.position.z >= -50) return true;
        if (zonaID == "Sur" && player.position.z < -50) return true;
        return false;
    }
}
