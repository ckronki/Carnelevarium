using NavKeypad;
using UnityEngine;
using UnityEngine.InputSystem.HID;
using System.Collections;

public class SimonManager : MonoBehaviour
{

    [Header("Game Setup")]
    [SerializeField] private int numRows = 3;
    [SerializeField] private int numCols = 4;
    private int numTiles;
    private Tile[] tile;

    [Header("Game Objects")]
    [SerializeField] private Tile tilePrefab;
    [SerializeField] private Transform gameArea;

    [Header("Audio Setup")]
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private AudioSource audioSource;

    public GameObject player;
    public GameObject simonSays;
    public GameObject hud;

    private bool _isResetting;

    [SerializeField] float timeToReset;
    [SerializeField] InteractionController interactionController;
    public SimonSaysOpen openSimonSays;

    public Door currentDoor;


    public void Update()
    {
        if (simonSays.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<Player>().enabled = false;
            interactionController.LockInteraction();

            if (GameManager.instance.player.hasCrowbar)
            {
                GameManager.instance.crowbarController.AttackLock();
            }
        }
        else
        {
            hud.SetActive(true);
            player.GetComponent<Player>().enabled = true;
            interactionController.UnlockInteraction();

            if (GameManager.instance.player.hasCrowbar)
            {
                GameManager.instance.crowbarController.AttackUnlock();
            }
        }
    }


    void Start()
    {
        // numTiles is global as we'll use it in lots of places.
        numTiles = numRows * numCols;
        tile = new Tile[numTiles];

        // Create the grid of tiles.
        for (int row = 0; row < numRows; row++)
        {
            for (int col = 0; col < numCols; col++)
            {
                int index = (row * numCols) + col;

                tile[index] = Instantiate(tilePrefab, gameArea);
                tile[index].Init(this, index, Color.HSVToRGB((float)index / numTiles, 0.8f, 0.9f));
            }
        }

        // Scale the tiles to fit our vertical space (6 units)
        // (If there are too many cols they'll go off the edge).
        float scale = 6f / numRows;
        gameArea.localScale = Vector3.one * scale;
    }

    private IEnumerator FlashTile(int index)
    {
        tile[index].TurnOn();
        yield return new WaitForSeconds(duration);
        tile[index].TurnOff();
    }

    public void PlayLightAndTone(int index)
    {
        StartCoroutine(FlashTile(index));
        PlayTone(index);
    }

    private void PlayTone(int index)
    {
        // Adjust pitch to create unique sound for each tile.
        if (numTiles > 1)
        {
            audioSource.pitch = Mathf.Lerp(0.5f, 2.0f, index / (numTiles - 1f));
        }

        // Schedule the tone to play.
        double currentTime = AudioSettings.dspTime;
        audioSource.PlayScheduled(currentTime);
        audioSource.SetScheduledEndTime(currentTime + duration);
    }
}
