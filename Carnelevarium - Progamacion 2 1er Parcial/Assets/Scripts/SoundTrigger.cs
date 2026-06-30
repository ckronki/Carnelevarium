using UnityEngine;

public class SoundTrigger : MonoBehaviour

{
    [Header("AudioSource que reproduce el sonido")]
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play(); // se reproduce una vez
            }
        }
    }
}
