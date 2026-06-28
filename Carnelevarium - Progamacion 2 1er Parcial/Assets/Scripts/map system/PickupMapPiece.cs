//TP2 - Sofia Liberman
// HERENCIA: PickupMapPiece hereda de MonoBehaviour, lo que le permite funcionar como componente en Unity.
// INTERFACES: implementa IInteractable, lo que obliga a definir el método Interact().

using UnityEngine;

public class PickupMapPiece : MonoBehaviour, IInteractable
{
    [Header("Nivel de mapa que desbloquea")]
    public int nivelMapa = 1;

    // Este método se llama automáticamente desde InteractionController
    public void Interact()
    {
        // Buscar el MapSystem y desbloquear el nivel
        MapSystem mapSystem = FindObjectOfType<MapSystem>();
        if (mapSystem != null)
        {
            mapSystem.AddPiece(nivelMapa);
        }

        // Ocultar el objeto después de recogerlo
        gameObject.SetActive(false);

        Debug.Log("Has recogido el mapa nivel " + nivelMapa);
    }
}
