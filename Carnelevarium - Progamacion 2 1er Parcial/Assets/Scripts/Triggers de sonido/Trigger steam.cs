using UnityEngine;

public class ParticleTrigger : MonoBehaviour
{
    public ParticleSystem particles; 
    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            if (particles != null)
                particles.Play(); //activa las partículas una sola vez
        }
    }
}
