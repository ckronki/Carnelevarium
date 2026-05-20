using UnityEngine;

public class Screamer : MonoBehaviour
{
    public GameObject screamer;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Player player = other.GetComponent<Player>();
            if (player.CurrentLife <= 0)
            {
                Instantiate(screamer);
            }
        }
    }
}
