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
    public OpenKeypad openKeypad;

    public Door currentDoor;

    public TMP_Text keypadText;
    public string currentKeypadAnswer;

    //public AudioSource button;
    //public AudioSource correct;
    //public AudioSource wrong;

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

        openKeypad.hasOpened = true;

        currentDoor.OpenDoor();
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

        Clear();
    }

    public void Clear()
    {
        keypadText.text = "";
        keypadText.color = Color.black;
    }
}
