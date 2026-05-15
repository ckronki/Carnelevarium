using UnityEngine;

public class OpenKeypad : MonoBehaviour , IInteractable
{
    [SerializeField] GameObject keypadUI;

    [SerializeField] string keypadAnswer;    

    public void Interact()
    {
        keypadUI.SetActive(true);
        keypadUI.GetComponent<Keypad>().SetAnswer(keypadAnswer);

        Debug.Log("Código actual: " + keypadUI.GetComponent<Keypad>().currentKeypadAnswer);
    }
}
