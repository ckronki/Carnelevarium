using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : ItemCheck , IInteractable
{
    [TextArea][SerializeField] string crowbarntDialogue;
    [TextArea][SerializeField] string fusentDialogue;
    [TextArea][SerializeField] string valventDialogue;

    [TextArea][SerializeField] string crowbarDialogue;
    [TextArea][SerializeField] string fuseDialogue;

    [TextArea][SerializeField] string winDialogue;

    [SerializeField] float dialogueTime;

    [SerializeField] Transform macarenaCam;
    [SerializeField] Camera playerCamera;

    public bool hasUsedCrowbar;
    public bool hasUsedFuse;
    public bool hasUsedValve;

    public void Update()
    {
        if (hasUsedValve == true)
        {
            GameManager.instance.crowbarController.AttackLock();
            GameManager.instance.player.CantMove();
            GameManager.instance.cameraController.LockCamera();

            playerCamera.transform.position = macarenaCam.position;
            playerCamera.transform.rotation = macarenaCam.rotation;

            UIManager.instance.dialogueText.text = winDialogue;
        }
    }

    public void Interact()
    {
        InventoryCheck();

        if (hasFoundItem)
        {
            if (!hasUsedCrowbar)
            {
                StartCoroutine(HasInteracted(crowbarDialogue, dialogueTime));

                hasUsedCrowbar = true;
                hasFoundItem = false;
            }
            else if (hasUsedCrowbar && !hasUsedFuse)
            {
                StartCoroutine(HasInteracted(fuseDialogue, dialogueTime));
                hasUsedFuse = true;
                hasFoundItem = false;
            }
            else if (hasUsedCrowbar && hasUsedFuse && !hasUsedValve)
            {
                hasUsedValve = true;
            }
        }
        else if (!hasFoundItem)
        {
            if (!hasUsedCrowbar)
            {
                StartCoroutine(HasInteracted(crowbarntDialogue, dialogueTime));
                return;
            }
            else if (hasUsedCrowbar && !hasUsedFuse)
            {
                StartCoroutine(HasInteracted(fusentDialogue, dialogueTime));
            }
            else if (hasUsedCrowbar && hasUsedFuse && !hasUsedValve)
            {
                StartCoroutine(HasInteracted(valventDialogue, dialogueTime));
            }
        }
    }

    public IEnumerator HasInteracted(string dialogue, float time)
    {
        UIManager.instance.dialogueText.text = dialogue;

        yield return new WaitForSeconds(time);

        UIManager.instance.dialogueText.text = "";
    }
}
