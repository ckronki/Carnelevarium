using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class OpenKeypad : MonoBehaviour , IInteractable
{
    [SerializeField] Door door;

    [SerializeField] string keypadAnswer;
    [SerializeField] float dialogueTime;

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
            StartCoroutine(HasOpened(dialogueTime));
            return;
        }
        else
        {
            keypadUI.SetActive(true);

            keypadUI.GetComponent<Keypad>().openKeypad = this;

            keypadUI.GetComponent<Keypad>().SetAnswer(keypadAnswer);
            keypadUI.GetComponent<Keypad>().currentDoor = door;
            
            Debug.Log("Código actual: " + keypadUI.GetComponent<Keypad>().currentKeypadAnswer);
        }
        
    }

    public IEnumerator HasOpened(float time)
    {
        dialogueText.text = "Batate y grabalo :3";

        yield return new WaitForSeconds (time);

        dialogueText.text = "";
    }

}
