using UnityEngine;
using UnityEngine.InputSystem;

public class FlashLight : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleAction;
    [SerializeField] private Light flashlight;

    private void OnEnable()
    {
        toggleAction.action.performed += OnToggle;
        toggleAction.action.Enable();
    }

    private void OnDisable()
    {
        toggleAction.action.performed -= OnToggle;
        toggleAction.action.Disable();
    }

    private void OnToggle(InputAction.CallbackContext ctx)
    {
        flashlight.enabled = !flashlight.enabled;
    }
}