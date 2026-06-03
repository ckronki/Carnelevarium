using UnityEngine;
using TMPro;

public class SimonSaysUI : MonoBehaviour
{
    public GameObject panel;
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI escapeHintText; // Texto pequeño tipo "ESC para salir"

    void Start()
    {
        if (escapeHintText != null)
            escapeHintText.text = "ESC para salir";
    }

    public void ShowPanel(bool show)
    {
        panel.SetActive(show);
    }

    public void SetMessage(string msg)
    {
        if (messageText) messageText.text = msg;
    }

    public void SetScore(int round)
    {
        if (scoreText) scoreText.text = $"Ronda {round}";
    }
}
