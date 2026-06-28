//TP2 - Sofia Liberman

using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    public string roomName; // Ej: "HabitacionNorte", "HabitacionCentro", "HabitacionSur"
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            MapSystem.Instance.SetCurrentRoom(roomName);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Solo limpiar si el jugador realmente salió de esta habitación
            if (MapSystem.Instance.GetCurrentRoom() == roomName)
            {
                MapSystem.Instance.SetCurrentRoom("");
            }
        }
    }

}
