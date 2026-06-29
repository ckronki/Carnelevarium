using UnityEngine;

public class CanTrigger : MonoBehaviour
{
    public Animator canAnimator;
    public AudioSource canSound;

    private bool triggered = false;

    [SerializeField] private Animator myAnimationController;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered && other.CompareTag("Player"))
        {
            triggered = true;

            
            myAnimationController.SetBool("roll2", true);

            //reproduce sonido
            if (canSound != null && !canSound.isPlaying)
                canSound.Play();
        }
    }
}