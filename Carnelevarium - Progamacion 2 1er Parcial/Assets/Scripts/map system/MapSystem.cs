// TP2 - Sofia Liberman
// COMPOSICIÓN: esta clase se utiliza dentro de MapSystem para representar zonas.
// ENCAPSULAMIENTO: variables privadas que no son accesibles directamente desde fuera de la clase

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

[System.Serializable]
public class ZonaData
{
    public GameObject zona;       
    public int nivelRequerido;    
}

public class MapSystem : MonoBehaviour
{
    public static MapSystem Instance;

    [Header("Mapas progresivos")]
    public GameObject mapa1;
    public GameObject mapa2;
    public GameObject mapa3;

    [Header("Mensaje sin mapa")]
    public GameObject mensajeSinMapa; 

    [Header("Zonas del mapa")]
    public ZonaData[] zonas; 

    [Header("Referencias del jugador")]
    public MonoBehaviour playerMovement;
    public MonoBehaviour playerCamera;

    private bool isOpen = false;
    private int mapaNivel = 0;
    private string currentRoom = "";

    [Header("Sonidos")]
    [SerializeField] AudioSource audioSource;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mapa1.SetActive(false);
        mapa2.SetActive(false);
        mapa3.SetActive(false);
        mensajeSinMapa.SetActive(false);

        foreach (ZonaData z in zonas)
        {
            z.zona.SetActive(false);
        }
    }


    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMap();

            if (mapaNivel != 0) audioSource.Play();
        }

        if (isOpen && mapaNivel > 0 && currentRoom != "")
        {
            ActualizarZonas();
        }
    }

    public void AddPiece(int nivel)
    {
        mapaNivel = nivel;
        Debug.Log("Has obtenido el mapa nivel " + nivel);
    }

    public void SetCurrentRoom(string roomName)
    {
        currentRoom = roomName;
    }

    public string GetCurrentRoom()
    {
        return currentRoom;
    }

    void ToggleMap()
    {
        isOpen = !isOpen;

        mapa1.SetActive(false);
        mapa2.SetActive(false);
        mapa3.SetActive(false);

        if (isOpen)
        {
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (playerMovement != null) playerMovement.enabled = false;
            if (playerCamera != null) playerCamera.enabled = false;

            if (mapaNivel == 0)
            {
                mensajeSinMapa.SetActive(true); 
            }
            else
            {
                mensajeSinMapa.SetActive(false);

                if (mapaNivel == 1) mapa1.SetActive(true);
                else if (mapaNivel == 2) mapa2.SetActive(true);
                else if (mapaNivel == 3) mapa3.SetActive(true);

                ActualizarZonas();
            }
        }
        else
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (playerMovement != null) playerMovement.enabled = true;
            if (playerCamera != null) playerCamera.enabled = true;

            mensajeSinMapa.SetActive(false);

            foreach (ZonaData z in zonas)
            {
                z.zona.SetActive(false);
            }
        }

    }

    void ActualizarZonas()
    {
        foreach (ZonaData z in zonas)
        {
            z.zona.SetActive(false);
        }

        if (!string.IsNullOrEmpty(currentRoom))
        {
            foreach (ZonaData z in zonas)
            {
                if (z.zona.name == currentRoom && mapaNivel >= z.nivelRequerido)
                {
                    z.zona.SetActive(true);

                    Image[] images = z.zona.GetComponentsInChildren<Image>();
                    foreach (Image img in images)
                    {
                        float alpha = Mathf.PingPong(Time.unscaledTime, 1f);
                        img.color = new Color(1f, 0f, 0f, alpha);
                    }
                }
            }
        }
    }
}
