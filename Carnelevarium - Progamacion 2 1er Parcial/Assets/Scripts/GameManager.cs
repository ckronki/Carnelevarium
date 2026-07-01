using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Player player;
    public StalkerMovement stalkerMovement;

    public CrowbarController crowbarController;
    public CameraController cameraController;
    

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
}
