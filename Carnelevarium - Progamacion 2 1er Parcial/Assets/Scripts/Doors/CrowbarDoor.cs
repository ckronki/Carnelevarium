using UnityEngine;

public class CrowbarDoor : Door
{
    [SerializeField] GameObject requiredItem;

    [TextArea][SerializeField] string openDialogue;

    public override void Interact()
    {
            if (!GameManager.instance.player.hasCrowbar)
            {
                StartCoroutine(HasInteracted(dialogue, dialogueTime));
                return;
            }
            else
            {
                OpenDoor();
                StartCoroutine(HasInteracted(openDialogue, dialogueTime));
            }
    }
}
