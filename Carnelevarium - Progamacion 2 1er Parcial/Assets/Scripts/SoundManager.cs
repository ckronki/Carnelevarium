using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip stepSound;
    public AudioClip attackSound;
    public AudioClip appearSound;

    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Demon|Walk2") && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(stepSound);
        }
        else if (state.IsName("Demon|Punch1") && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(attackSound);
        }
        else if (state.IsName("Demon|Come-out1") && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(appearSound);
        }
        
    }

}
