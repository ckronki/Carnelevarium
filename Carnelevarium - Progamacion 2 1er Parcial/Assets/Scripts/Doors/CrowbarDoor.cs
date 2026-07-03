using UnityEngine;

public class CrowbarDoor : Door
{
    [SerializeField] GameObject requiredItem;

    [TextArea][SerializeField] string openDialogue;

    [SerializeField] AudioSource stalkerEntry;

    public override void Interact()
    {
        if (!GameManager.instance.player.hasCrowbar)
        {
            StartCoroutine(HasInteracted(dialogue, dialogueTime));

            return;
        }
        else if (!GameManager.instance.stalkerMovement.canMove)
        {
            OpenDoor();
            StartCoroutine(HasInteracted(openDialogue, dialogueTime));

            stalkerEntry.Play();

            GameManager.instance.stalkerMovement.MovementState();
        }
    }
}
