using UnityEngine;
using UnityEngine.EventSystems;

public class TransformButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Materiales del botón")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private Material defaultState;
    [SerializeField] private Material hoverState;
    [SerializeField] private Material pressedState;
    [SerializeField] private Material disabledState;

    [Header("Estado del botón")]
    [SerializeField] private bool isDisabled = false;

    void Start()
    {
        if (targetRenderer != null)
            targetRenderer.material = defaultState;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isDisabled && targetRenderer != null)
            targetRenderer.material = hoverState;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isDisabled && targetRenderer != null)
            targetRenderer.material = defaultState;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isDisabled && targetRenderer != null)
            targetRenderer.material = pressedState;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isDisabled && targetRenderer != null)
            targetRenderer.material = hoverState;
    }

    public void SetDisabled(bool value)
    {
        isDisabled = value;
        if (targetRenderer != null)
            targetRenderer.material = isDisabled ? disabledState : defaultState;
    }
}
