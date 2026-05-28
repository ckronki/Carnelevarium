using UnityEngine;
using UnityEngine.InputSystem;

public class ClickManager : MonoBehaviour
{
    public Camera mainCamera;
    private MenuTextButton lastHovered;

    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            MenuTextButton btn = hit.collider.GetComponent<MenuTextButton>();
            if (btn != null)
            {
                if (lastHovered != btn)
                {
                    if (lastHovered != null)
                    {
                        lastHovered.OnHoverExit();
                        Debug.Log("Mouse dejó de estar sobre: " + lastHovered.action);
                    }
                    btn.OnHoverEnter();
                    lastHovered = btn;
                }
            }
        }
        else
        {
            if (lastHovered != null)
            {
                lastHovered.OnHoverExit();
                Debug.Log("Mouse salió del botón: " + lastHovered.action);
                lastHovered = null;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame && lastHovered != null)
        {
            Debug.Log("Click detectado en botón: " + lastHovered.action);
            lastHovered.ExecuteAction();
        }
    }
}
