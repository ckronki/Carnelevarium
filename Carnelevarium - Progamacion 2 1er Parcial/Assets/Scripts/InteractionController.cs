using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class InteractionController : MonoBehaviour
{
    [SerializeField] Camera _playerCamera;
    public CameraController _cameraController;
    [SerializeField] float _interactionDistance;
    [SerializeField] float _interactionDistanceBackup;
    [SerializeField] GameObject _interactionCrosshair;
    IInteractable _currentTargetedInteractable;

    public void Start()
    {
        _interactionDistanceBackup = _interactionDistance;
    }

    public void Update()
    {
        UpdateCurrentInteractable();
        UpdateInteractionCrosshair();
        CheckForInteractionInput();
    }

    public void UpdateCurrentInteractable()
    {
        var ray = _playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));

        Physics.Raycast(ray, out var hit, _interactionDistance);

        _currentTargetedInteractable = hit.collider?.GetComponent<IInteractable>();
    }

    public void UpdateInteractionCrosshair()
    {
        if (_currentTargetedInteractable == null)
        {
            _interactionCrosshair.SetActive(false);
            return;
        }
        else
        {
            _interactionCrosshair.SetActive(true);
        }
    }

    public void CheckForInteractionInput()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame && _currentTargetedInteractable != null)
        {
            _currentTargetedInteractable.Interact();
        }
    }



    public void LockInteraction()
    {
        Debug.Log("Cámara bloqueada: " + _cameraController.isCameraLocked);
        _cameraController.LockCamera();

        _interactionDistance = 0;


    }

    public void UnlockInteraction()
    {
        Debug.Log("Cámara bloqueada: " + _cameraController.isCameraLocked);
        _cameraController.UnlockCamera();

        _interactionDistance = _interactionDistanceBackup;

    }
}
