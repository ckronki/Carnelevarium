using UnityEngine;

public class SafeRoom : MonoBehaviour
{
    public void OnTriggerEnter(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && !script.isPlayerInSafeRoom)
        {
            Debug.Log("El player entró a la safe room");
            script.SafeRoomState();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && script.isPlayerInSafeRoom)
        {
            Debug.Log("El player salió de la safe room");
            script.SafeRoomState();
        }
    }
}
