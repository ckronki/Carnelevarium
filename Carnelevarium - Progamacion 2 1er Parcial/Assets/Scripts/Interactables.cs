using System.Collections;
using UnityEditor.Rendering;
using UnityEngine;

public class Interactables : MonoBehaviour
{
    [TextArea][SerializeField] protected string dialogue;
    [SerializeField] protected float dialogueTime;

    public IEnumerator HasInteracted(string dialogue, float time)
    {
        UIManager.instance.dialogueText.text = dialogue;

        yield return new WaitForSeconds(time);

        UIManager.instance.dialogueText.text = "";
    }
}
