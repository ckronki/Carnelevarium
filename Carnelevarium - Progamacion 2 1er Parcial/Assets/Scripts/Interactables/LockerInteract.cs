using UnityEngine;

public class LockerInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Animator lockerAnimator;
    [SerializeField] private GameObject lockerContents;
    private bool isOpen = false;

    public void Interact()
    {
        if (!isOpen)
        {
            lockerAnimator.Play("anim_locker_open_close", 0, 0f); 
            if (lockerContents != null)
                lockerContents.SetActive(true);
            isOpen = true;
        }
        else
        {
            lockerAnimator.Play("anim_locker_open_close", 0, 0.5f); 
            if (lockerContents != null)
                lockerContents.SetActive(false);
            isOpen = false;
        }
    }
}
