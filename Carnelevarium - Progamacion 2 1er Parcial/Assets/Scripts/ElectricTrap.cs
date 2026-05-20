using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ElectricTrap : MonoBehaviour
{
    [Header("Ajustes de Daño")]
    [SerializeField] private int damageAmount = 5;
    [SerializeField] private float damageTickRate = 2f;

    [Header("Ajustes de Ralentización")]
    [SerializeField] private float slowMultiplier = 0.5f;

    private Dictionary<Player, Coroutine> activeTraps = new Dictionary<Player, Coroutine>();
    private Dictionary<Player, float> originalSpeeds = new Dictionary<Player, float>();

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && !activeTraps.ContainsKey(player))
        {
            // Detener sprint antes de ralentizar
            player.ForceStopSprint();

            // Guardar la velocidad base antes de ralentizar
            originalSpeeds[player] = player.CurrentSpeed;

            // Aplicar ralentización
            player.ChangeSpeed(slowMultiplier);

            // Bloquear el sprint mientras está en la trampa
            player.DisableSprintTemporarily(Mathf.Infinity);

            // daño constante
            Coroutine damageRoutine = StartCoroutine(ApplyElectricDamage(player));
            activeTraps.Add(player, damageRoutine);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null && activeTraps.ContainsKey(player))
        {
            // stop daño
            StopCoroutine(activeTraps[player]);
            activeTraps.Remove(player);

            // Restaurar velocidad original
            if (originalSpeeds.ContainsKey(player))
            {
                player.ResetSpeed(originalSpeeds[player]);
                originalSpeeds.Remove(player);
            }

            // Permitir sprint de nuevo
            player.EnableSprint(); // Debes agregar este método en Player
        }
    }

    private IEnumerator ApplyElectricDamage(Player player)
    {
        while (player != null)
        {
            player.GetDamage(damageAmount);
           
            // shakecamara
            if (ShakeCamara.Instance != null) ShakeCamara.Instance.Shake(0.3f);
            yield return new WaitForSeconds(damageTickRate);

        }
    }
}