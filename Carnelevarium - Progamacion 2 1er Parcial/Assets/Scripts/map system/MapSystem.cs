using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MapSystem : MonoBehaviour
{
    public static MapSystem Instance { get; private set; }

    public GameObject mapa1;
    public GameObject mapa2;
    public GameObject mapa3;
    public TextMeshProUGUI mensajeTMP;

    [Header("Referencia al Player")]
    public Player playerMovement;
    public CameraController playerCamera;

    [Header("Zonas del mapa")]
    public Image[] zonas; // todas las imágenes rojas en el objeto Zonas

    private int mapaNivel = 0;
    private bool isOpen = false;
    private string currentRoom = "";

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        mapa1.SetActive(false);
        mapa2.SetActive(false);
        mapa3.SetActive(false);
        mensajeTMP.gameObject.SetActive(false);

        foreach (Image zona in zonas)
        {
            zona.enabled = false;
        }
    }
    public string GetCurrentRoom()
    {
        return currentRoom;
    }

    void Update()
    {
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            ToggleMap();
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
                mensajeTMP.text = "No tienes ningún mapa";
                mensajeTMP.gameObject.SetActive(true);
            }
            else
            {
                mensajeTMP.gameObject.SetActive(false);

                if (mapaNivel == 1) mapa1.SetActive(true);
                else if (mapaNivel == 2) mapa2.SetActive(true);
                else if (mapaNivel == 3) mapa3.SetActive(true);

                // ?? Mostrar la zona actual encima del mapa
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

            mensajeTMP.gameObject.SetActive(false);

            foreach (Image zona in zonas)
            {
                zona.enabled = false;
                zona.color = Color.red;
            }
        }
    }

    void ActualizarZonas()
    {
        foreach (Image zona in zonas)
        {
            zona.enabled = false;
        }

        if (!string.IsNullOrEmpty(currentRoom))
        {
            foreach (Image zona in zonas)
            {
                if (zona.name == currentRoom)
                {
                    zona.enabled = true;
                    zona.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.unscaledTime, 1));
                }
            }
        }
    }

}
