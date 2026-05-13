using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypad;
    public GameObject hud;

    [SerializeField] InteractionController interactionController;
    [SerializeField] GameObject openKeypad;

    public GameObject door;
    public Animator doorAnimator;

    public TextMeshProUGUI keypadText;
    [SerializeField] string keypadAnswer = "12345";

    //public AudioSource button;
    //public AudioSource correct;
    //public AudioSource wrong;

    public bool animate;

    public void Start()
    {
        //keypad.SetActive(false);
    }

    public void Number(int number)
    {
        keypadText.text += number.ToString();
    }

    public void Execute()
    {
        if (keypadText.text == keypadAnswer)
        {
            keypadText.text = "Right";
            animate = true;
        }
        else
        {
            keypadText.text = "Wrong";
        }
    }

    public void Clear()
    {
        keypadText.text = "";

    }

    public void Exit()
    {
        keypad.SetActive(false);
        hud.SetActive(true);
        player.GetComponent<Player>().enabled = true;
        interactionController.UnlockInteraction();
        openKeypad.GetComponent<OpenKeypad>().enabled = false;
    }

    public void Update()
    {
        if (keypadText.text == "Right" && animate)
        {
            doorAnimator.SetBool("Open", true);
            Debug.Log("Door opens");
            Exit();
        }

        if (keypad.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<Player>().enabled = false;
            interactionController.LockInteraction();
        }
    }
}
