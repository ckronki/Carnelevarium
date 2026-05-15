using UnityEngine;

public class RightButton : SecurityCameraSystem , IInteractable
{
    [SerializeField] SecurityCameraSystem leftButton;

    public void Interact()
    {
        NextCam();
    }
    private void NextCam()
    {
        _cameraSelected += 1;

        if (_cameraSelected > _cameraList.Length - 1)
        {
            _cameraSelected = 0;
        }

        if (_cameraSelected > 0)
        {
            _cameraList[_cameraSelected - 1].SetActive(false);
        }

        if (_cameraSelected == 0)
        {
            _cameraList[_cameraList.Length - 1].SetActive(false);
        }

        _cameraList[_cameraSelected].SetActive(true);
        leftButton._cameraSelected = _cameraSelected;
        Debug.Log("Cámara seleccionada: " + (_cameraSelected + 1));
    }
}
