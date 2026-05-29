using UnityEngine;

public class SimpleSliderToggle : MonoBehaviour
{
    public GameObject optionsSlider;   // tu Slider

    // Mostrar el slider cuando entres al menú de opciones
    public void ShowSlider()
    {
        if (optionsSlider) optionsSlider.SetActive(true);
    }

    // Ocultar el slider cuando vuelvas al menú principal
    public void HideSlider()
    {
        if (optionsSlider) optionsSlider.SetActive(false);
    }
}
