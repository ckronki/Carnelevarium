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
    public int currentCodeLimit;

    public AudioSource audioSource; 
    public AudioClip correct;
    public AudioClip wrong;
    public AudioClip clicking;

    public void Update()
    {
        if (keypad.activeInHierarchy)
        {
            hud.SetActive(false);
            player.GetComponent<Player>().enabled = false;
            interactionController.LockInteraction();

            if (GameManager.instance.player.hasCrowbar)
            {
                GameManager.instance.crowbarController.AttackLock();
            }
        }
    }

    public void SetAnswer(string answer)
    {
        currentKeypadAnswer = answer;
        keypadText.text = "";
    }
    
    public void SetLimit(int limit)
    {
        currentCodeLimit = limit;
    }

    public void Number(int number)
    {
        if (keypadText.text.Length >= currentCodeLimit)
        {
            return;
        }
        else if (!_isResetting)
        {
            keypadText.text += number.ToString();
        }
    }

    public IEnumerator Right(float time)
    {
        if (audioSource != null)
        {
            audioSource.PlayOneShot(correct);
        }

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
        if (audioSource != null)
        {
            audioSource.PlayOneShot(wrong);
        }

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

        if (GameManager.instance.player.hasCrowbar)
        {
            GameManager.instance.crowbarController.AttackUnlock();
        }

        Clear();
    }

    public void Clear()
    {
        keypadText.text = "";
        keypadText.color = Color.black;
    }

}
