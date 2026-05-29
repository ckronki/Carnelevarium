using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public AudioSource audioSource;

    void Start()
    {
        // Inicializar con el valor actual
        volumeSlider.value = audioSource.volume;
        // Suscribirse al evento
        volumeSlider.onValueChanged.AddListener(OnVolumeChange);
    }

    void OnVolumeChange(float value)
    {
        audioSource.volume = value;
        Debug.Log("Volumen cambiado a: " + value);
    }
}
