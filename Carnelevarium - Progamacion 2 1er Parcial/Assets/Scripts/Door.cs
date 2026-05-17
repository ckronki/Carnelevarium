using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    public Animator door;
    public GameObject doorInteractionArea;
    public bool isOpen;
    [SerializeField] bool _isOpenable;

    public void Interact()
    {
        if (!_isOpenable)
        {
            Debug.Log("Batate y grabalo :3");
        }
        else
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        Debug.Log("Se abre la puerta");
        door.SetBool("Open", true);
        doorInteractionArea.GetComponent<BoxCollider>().enabled = false;
    }
}
