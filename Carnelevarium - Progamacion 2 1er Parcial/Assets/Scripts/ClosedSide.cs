using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class ClosedSide : Interactables , IInteractable
{
    [SerializeField] Door _door;

    public void Interact()
    {
        StartCoroutine(HasInteracted(dialogue, dialogueTime));
    }

    public void Update()
    {
        if (!_door.isOpen)
        {
            return;
        }
        else
        {
            this.GetComponent<BoxCollider>().enabled = false;
        }
    }

    public IEnumerator HasInteracted(float time)
    {
        UIManager.instance.dialogueText.text = dialogue;

        yield return new WaitForSeconds(time);

        UIManager.instance.dialogueText.text = dialogue;
    }
}
