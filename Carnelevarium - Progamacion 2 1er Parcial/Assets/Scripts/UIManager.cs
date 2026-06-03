using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject keypadUI;
    public GameObject simonSaysUI;
    public TMP_Text dialogueText;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else Destroy(gameObject);
    }
}
