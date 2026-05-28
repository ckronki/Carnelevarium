using UnityEngine;
using TMPro;

public class MenuTextButton : MonoBehaviour
{
    public string action;
    private TextMeshPro textMesh;
    private Color originalColor;
    public bool isActive = false; // nuevo flag

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        originalColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f);
    }

    public void ExecuteAction()
    {
        if (!isActive) return; // ignora clicks si no está activo

        MenuGameManager gm = FindObjectOfType<MenuGameManager>();
        if (action == "Options") gm.ShowOptions();
        else if (action == "Exit") gm.ExitGame();
        else if (action == "Back") gm.BackToMenu();
        else if (action == "Play") gm.PlayGame();

        Debug.Log("Ejecutando acción: " + action);
    }

    public void OnHoverEnter()
    {
        if (!isActive) return; // ignora hover si no está activo
        textMesh.color = Color.yellow;
        Debug.Log("Mouse sobre botón: " + action);
    }

    public void OnHoverExit()
    {
        if (!isActive) return; // ignora hover si no está activo
        if (textMesh.color.a > 0.5f)
        {
            textMesh.color = originalColor;
            Debug.Log("Mouse salió del botón: " + action);
        }
    }
}
