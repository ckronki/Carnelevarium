using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class OpenKeypad : Interactables , IInteractable
{
    [SerializeField] Door door;

    [SerializeField] string keypadAnswer;

    [SerializeField] int codeLimit;

    private GameObject keypadUI;
    private TMP_Text dialogueText;

    public void Start()
    {
        keypadUI = UIManager.instance.keypadUI;
        dialogueText = UIManager.instance.dialogueText;
    }

    public bool hasOpened;

    public void Interact()
    {
        if (hasOpened)
        {
            StartCoroutine(HasInteracted(dialogue, dialogueTime));
            return;
        }
        else
        {
            keypadUI.SetActive(true);

            keypadUI.GetComponent<Keypad>().openKeypad = this;

            keypadUI.GetComponent<Keypad>().SetAnswer(keypadAnswer);
            keypadUI.GetComponent<Keypad>().SetLimit(codeLimit);
            keypadUI.GetComponent<Keypad>().currentDoor = door;
            
            Debug.Log("Código actual: " + keypadUI.GetComponent<Keypad>().currentKeypadAnswer);
        }
    }
}
