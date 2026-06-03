using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    private SimonManager gameManager;
    private Image imageRenderer;
    private int tileId;
    private Color colour;

    public void Init(SimonManager gameManager, int tileId, Color colour)
    {
        this.gameManager = gameManager;
        this.tileId = tileId;
        this.colour = colour;

        imageRenderer = GetComponent<Image>();
        TurnOff();

        // Conectar el click del botón al método OnClick
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void TurnOff()
    {
        imageRenderer.color = colour * 0.3f;
    }

    public void TurnOn()
    {
        imageRenderer.color = colour;
    }

    private void OnClick()
    {
        gameManager.PlayLightAndTone(tileId);
        Debug.Log("Tile clickeado: " + tileId);
    }
}
