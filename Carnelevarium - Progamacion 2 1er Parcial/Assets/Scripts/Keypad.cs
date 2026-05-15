using System.Collections;
using TMPro;
using Unity.Multiplayer.Center.Common;
using UnityEngine;
using UnityEngine.UI;

public class Keypad : MonoBehaviour
{
    public GameObject player;
    public GameObject keypad;
    public GameObject hud;

    private bool _isResetting;

    [SerializeField] float timeToReset;
    [SerializeField] InteractionController interactionController;
    [SerializeField] GameObject openKeypad;

    public GameObject door;
    public Animator doorAnimator;

    public TMP_Text keypadText;
    public string currentKeypadAnswer;

    //public AudioSource button;
    //public AudioSource correct;
    //public AudioSource wrong;

    public void Start()
    {
        keypad.SetActive(false);
    }

    public void Update()
    {
        if (keypad.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<Player>().enabled = false;
            interactionController.LockInteraction();
        }
    }

    public void SetAnswer(string answer)
    {
        currentKeypadAnswer = answer;
        keypadText.text = "";
    }

    public void Number(int number)
    {
        if (!_isResetting)
        {
            keypadText.text += number.ToString();
        }
    }

    public IEnumerator Right(float time)
    {
        keypadText.text = "Right";
        keypadText.color = Color.green;
        _isResetting = true;

        yield return new WaitForSeconds(time);

        Exit();
        OpenAnimation();
        _isResetting = false;
    }

    public IEnumerator Wrong(float time)
    {
        keypadText.text = "Wrong";
        keypadText.color = Color.red;
        _isResetting = true;

        yield return new WaitForSeconds(time);

        Clear();
        _isResetting = false;
    }

    public void Enter()
    {
        if (keypadText.text == currentKeypadAnswer)
        {
            StartCoroutine(Right(timeToReset));
        }
        else
        {
            StartCoroutine(Wrong(timeToReset));
        }
    }

    public void Exit()
    {
        keypad.SetActive(false);
        hud.SetActive(true);
        player.GetComponent<Player>().enabled = true;
        interactionController.UnlockInteraction();

        openKeypad.GetComponent<OpenKeypad>().enabled = false;

        Clear();
    }

    public void Clear()
    {
        keypadText.text = "";
        keypadText.color = Color.black;
    }

    public void OpenAnimation()
    {
        doorAnimator.SetBool("Open", true);
        Debug.Log("Door opens");
    }
}
