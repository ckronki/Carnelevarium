using System.Collections;
using UnityEngine;

public class PickupCrowbar : Interactables, IInteractable
{
    public bool isCrowbar;

    public void Interact()
    {
        Inventory.instance.AddItem(this.name);

        StartCoroutine(HasInteracted(dialogue, dialogueTime));

        
        if (!GameManager.instance.player.hasCrowbar && isCrowbar)
        {
            GameManager.instance.player.GetCrowbar();
            GameManager.instance.crowbarController.AttackUnlock();
        }
    }
}
