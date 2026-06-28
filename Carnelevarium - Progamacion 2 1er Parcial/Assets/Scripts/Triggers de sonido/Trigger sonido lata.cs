using UnityEngine;

public class CanTrigger : MonoBehaviour
{
    public Animator canAnimator;     
    public AudioSource canSound;     

    private bool triggered = false;

    [SerializeField] private Animator myAnimationController;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag ("Player"))
        {

            myAnimationController.SetBool("roll2", true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {

            myAnimationController.SetBool("roll2", false);

        }
    }
}