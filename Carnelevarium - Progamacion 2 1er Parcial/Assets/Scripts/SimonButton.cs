using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SimonButton : MonoBehaviour
{
    [Header("Colores")]
    public Color normalColor;
    public Color flashColor;
    public Color errorColor = Color.red;

    [Header("Referencias")]
    public int buttonIndex;
    public Image buttonImage;
    public SimonSaysManager manager;

    private bool interactable = false;

    public void SetInteractable(bool value)
    {
        interactable = value;
        buttonImage.color = value ? normalColor : normalColor * 0.7f;
    }

    public void OnClick()
    {
        if (!interactable) return;
        manager.OnButtonPressed(buttonIndex);
        StartCoroutine(Flash(0.2f));
    }

    public IEnumerator Flash(float duration)
    {
        buttonImage.color = flashColor;
        yield return new WaitForSeconds(duration);
        buttonImage.color = normalColor;
    }

    public IEnumerator FlashError()
    {
        for (int i = 0; i < 3; i++)
        {
            buttonImage.color = errorColor;
            yield return new WaitForSeconds(0.15f);
            buttonImage.color = normalColor;
            yield return new WaitForSeconds(0.1f);
        }
    }
}
