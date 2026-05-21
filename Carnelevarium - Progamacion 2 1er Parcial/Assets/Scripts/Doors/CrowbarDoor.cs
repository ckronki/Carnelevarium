using UnityEngine;

public class CrowbarDoor : Door
{
    [SerializeField] GameObject requiredItem;

    [TextArea][SerializeField] string openDialogue;

    public override void Interact()
    {
        Debug.Log(requiredItem.name);
        for (int i = 0; i < Inventory.instance.items.Count; i++)
        {
            Debug.Log("Entró al loop");
            Debug.Log(Inventory.instance.items[i]);
            if (Inventory.instance.items[i] != requiredItem.name)
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
}
