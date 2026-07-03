using UnityEngine;

public class CrowbarSound : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    public void SwooshSound()
    {
        audioSource.Play();
    }
}
