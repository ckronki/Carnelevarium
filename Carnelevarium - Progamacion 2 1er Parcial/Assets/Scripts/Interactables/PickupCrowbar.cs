using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PickupCrowbar : Interactables, IInteractable
{
    public bool isCrowbar;

    public void Interact()
    {
        Inventory.instance.AddItem(this.name);

        StartCoroutine(IsInteracting());

        
    }

    public IEnumerator IsInteracting()
    {
        StartCoroutine(HasInteracted(dialogue, dialogueTime));

        this.gameObject.GetComponent<Collider>().enabled = false;

        yield return new WaitForSeconds(dialogueTime);

        this.gameObject.SetActive(false);

        if (!GameManager.instance.player.hasCrowbar && isCrowbar)
        {
            GameManager.instance.player.GetCrowbar();
            GameManager.instance.crowbarController.AttackUnlock();
        }
    }
}
