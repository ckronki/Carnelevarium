using UnityEngine;

public class LeftButton : SecurityCameraSystem, IInteractable
{
    [SerializeField] SecurityCameraSystem rightButton;

    public void Interact()
    {
        PreviousCam();
    }

    private void PreviousCam()
    {
        _cameraSelected -= 1;

        if (_cameraSelected < 0)
        {
            _cameraSelected = _cameraList.Length - 1;
        }

        if (_cameraSelected == _cameraList.Length - 1)
        {
            _cameraList[0].SetActive(false);
        }

        if (_cameraSelected < _cameraList.Length - 1)
        {
            _cameraList[_cameraSelected + 1].SetActive(false);
        }

        _cameraList[_cameraSelected].SetActive(true);
        rightButton._cameraSelected = _cameraSelected;
        Debug.Log("Cámara seleccionada: " + (_cameraSelected + 1));
    }
}
