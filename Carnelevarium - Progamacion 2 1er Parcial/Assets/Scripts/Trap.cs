using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float freezeDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            // Congelar al jugador
            player.Freeze(freezeDuration);

            Destroy(gameObject);

            // Cámara shake
            ShakeCamara.Instance.Shake(freezeDuration);
        }
    }
}
