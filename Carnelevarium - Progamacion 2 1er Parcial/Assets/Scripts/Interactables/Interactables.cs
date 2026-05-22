using System.Collections;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    [TextArea][SerializeField] protected string dialogue;
    [SerializeField] protected float dialogueTime;

    public IEnumerator HasInteracted(string dialogue, float time)
    {
        UIManager.instance.dialogueText.text = dialogue;
        Debug.Log("Comenzó la coroutine");

        yield return new WaitForSeconds(time);

        Debug.Log("Se terminó la coroutine");
        UIManager.instance.dialogueText.text = "";
    }
}
