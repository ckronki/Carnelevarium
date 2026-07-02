using UnityEngine;

public class SoundTrigger : MonoBehaviour

{
    [Header("AudioSource que reproduce el sonido")]
    public AudioSource audioSource;
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (audioSource != null && !audioSource.isPlaying)
            {
                audioSource.Play(); // se reproduce una vez
            }
        }
    }
}
