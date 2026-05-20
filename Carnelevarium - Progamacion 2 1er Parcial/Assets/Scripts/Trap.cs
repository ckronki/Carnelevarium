using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private float freezeDuration = 2f;

    private void OnTriggerEnter(Collider other)
    {
        
        Player player = other.GetComponent<Player>();
        if (player != null)
        {
            player.setStun();
            player.Freeze(freezeDuration);

            Destroy(gameObject);

            
            ShakeCamara.Instance.Shake(freezeDuration);
        }
    }
}
