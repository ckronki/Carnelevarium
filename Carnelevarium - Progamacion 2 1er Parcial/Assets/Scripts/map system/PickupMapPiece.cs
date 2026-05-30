using UnityEngine;
using UnityEngine.InputSystem;

public class PickupMapPiece : MonoBehaviour
{
    public int nivelMapa; // 1, 2 o 3
    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            MapSystem mapSystem = FindObjectOfType<MapSystem>();
            if (mapSystem != null)
            {
                mapSystem.AddPiece(nivelMapa); // ? ahora sí existe
            }

            gameObject.SetActive(false); // ocultar cubo
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) playerInRange = false;
    }
}
