using UnityEngine;

public class LowHealthEffect : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private Material effectMaterial;

    [Header("Vida Baja")]
    [SerializeField] private int lowHealthThreshold = 20;

    [Header("Valores del efecto")]
    [SerializeField] private float normalIntensity = 0f;
    [SerializeField] private float lowHealthIntensity = 6f;

    private void Update()
    {
        if (player == null || effectMaterial == null)
            return;

        if (player.CurrentLife <= lowHealthThreshold)
        {
            effectMaterial.SetFloat("_VignetteIntensity", lowHealthIntensity);
        }
        else
        {
            effectMaterial.SetFloat("_VignetteIntensity", normalIntensity);
        }
    }
}
