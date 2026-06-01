using UnityEngine;

public class PuzzleActivator3D : MonoBehaviour
{
    [SerializeField] private GameObject puzzle3D; // referencia al puzzle en la escena

    private void OnMouseDown()
    {
        // Al hacer clic sobre el objeto 3D
        puzzle3D.SetActive(true);
    }
}
