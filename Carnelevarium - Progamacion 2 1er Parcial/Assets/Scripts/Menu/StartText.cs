using UnityEngine;
using TMPro;

public class StartText : MonoBehaviour
{
    public TextMeshPro textMesh;

    void Start()
    {
        textMesh.gameObject.SetActive(true);
    }

    public void HideText()
    {
        textMesh.gameObject.SetActive(false);
    }
}
