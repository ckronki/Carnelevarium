//TP2 - Sofia Liberman
// HERENCIA: PickupMapPiece hereda de MonoBehaviour, lo que le permite funcionar como componente en Unity.
// INTERFACES: implementa IInteractable, lo que obliga a definir el método Interact().
using UnityEngine;

public class PickupMapPiece : MonoBehaviour, IInteractable
{
    [Header("Nivel de mapa que desbloquea")]
    public int nivelMapa = 1;

    [Header("Sonido al recoger")]
    public AudioClip pickupSound; // asignar en Inspector

    public void Interact()
    {
        // Buscar el MapSystem y desbloquear el nivel
        MapSystem mapSystem = FindObjectOfType<MapSystem>();
        if (mapSystem != null)
        {
            mapSystem.AddPiece(nivelMapa);
        }

        // Reproducir sonido desde el SoundManager
        if (SoundsManager.Instance != null)
        {
            SoundsManager.Instance.PlaySound(pickupSound);
        }

        // Ocultar el objeto después de recogerlo
        gameObject.SetActive(false);

        Debug.Log("Has recogido el mapa nivel " + nivelMapa);
    }
}
