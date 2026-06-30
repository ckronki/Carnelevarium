using UnityEngine;

public class SoundsManager : MonoBehaviour
{
    public static SoundsManager Instance; // Singleton
    private AudioSource audioSource;

    void Awake()
    {
        // Creamos el singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Método para reproducir un sonido
    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
