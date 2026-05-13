using UnityEngine;

public class OpenKeypad : MonoBehaviour , IInteractable
{
    [SerializeField] GameObject keypad;

    public void Interact()
    {
        keypad.SetActive(true);
    }
}
