using UnityEngine;

public class Door : Interactables , IInteractable
{
    public Animator door;
    public BoxCollider doorInteractionArea;
    public bool isOpen;

    [SerializeField] bool _isOpenable;

    public void Interact()
    {
        if (!_isOpenable)
        {
            StartCoroutine(HasInteracted(dialogue, dialogueTime));
            return;
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Debug.Log("Se abre la puerta");
        door.SetBool("Open", true);

        doorInteractionArea.enabled = false;

        isOpen = true; 
    }
}
