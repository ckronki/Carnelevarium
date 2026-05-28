using UnityEngine;
using TMPro;

public class MenuTextButton : MonoBehaviour
{
    public string action;
    private TextMeshPro textMesh;
    private Color originalColor;

    void Start()
    {
        textMesh = GetComponent<TextMeshPro>();
        // fuerza alpha = 1 para que nunca se guarde transparente
        originalColor = new Color(textMesh.color.r, textMesh.color.g, textMesh.color.b, 1f);
    }

    public void ExecuteAction()
    {
        MenuGameManager gm = FindObjectOfType<MenuGameManager>();
        if (action == "Options") gm.ShowOptions();
        else if (action == "Exit") gm.ExitGame();
        else if (action == "Back") gm.BackToMenu();
        else if (action == "Play") gm.PlayGame();

        Debug.Log("Ejecutando acción: " + action);
    }

    public void OnHoverEnter()
    {
        textMesh.color = Color.yellow;
        Debug.Log("Mouse sobre botón: " + action);
    }

    public void OnHoverExit()
    {
        // Solo restaurar color si el texto está visible (alpha > 0.5)
        if (textMesh.color.a > 0.5f)
        {
            textMesh.color = originalColor;
            Debug.Log("Mouse salió del botón: " + action);
        }
    }

}
