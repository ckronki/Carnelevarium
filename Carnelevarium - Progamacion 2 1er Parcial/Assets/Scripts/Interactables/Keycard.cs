using UnityEngine;

public class Keycard : Interactables , IInteractable
{
    public void Interact()
    {
        StartCoroutine(HasInteracted(dialogue, dialogueTime));
    }
}
