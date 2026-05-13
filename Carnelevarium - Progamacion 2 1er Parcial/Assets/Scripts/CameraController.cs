using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public InputActionReference controlLook;
    public Transform playerTransform;
    private Vector2 _lookInput;
    private float _upDown;
    private float _leftRight;
    public float sensitivity;

    public bool isCameraLocked;

    private void Update()
    {
        if (isCameraLocked == true)
        {
            return;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;

            _lookInput = controlLook.action.ReadValue<Vector2>();

            _upDown -= _lookInput.y * sensitivity;
            _leftRight += _lookInput.x * sensitivity;

            _upDown = Mathf.Clamp(_upDown, -80f, 80f);
            transform.rotation = Quaternion.Euler(_upDown, _leftRight, 0);
            playerTransform.rotation = Quaternion.Euler(0, _leftRight, 0);
        }
    }

    public void UnlockCamera()
    {
        isCameraLocked = false;
    }
    public void LockCamera()
    {
        isCameraLocked = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
