using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    public Animator door;
    public GameObject doorInteractionArea;
    public bool isOpen;

    public void Interact()
    {
        OpenDoor();
    }

    public void OpenDoor()
    {
        Debug.Log("Se abre la puerta");
        door.SetBool("Open", true);
        doorInteractionArea.SetActive(false);
    }
}
