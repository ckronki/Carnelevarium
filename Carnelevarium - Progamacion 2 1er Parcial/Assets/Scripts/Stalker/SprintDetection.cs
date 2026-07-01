using UnityEngine;

public class SprintDetection : MonoBehaviour
{
    private StalkerMovement stalkerReference;
    void Start()
    {
        stalkerReference = GameManager.instance.stalkerMovement;
    }

    void Update()
    {
        this.transform.position = stalkerReference.transform.position;
    }

    public void OnTriggerEnter(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && script.isSprinting)
        {
            Debug.Log("El player entró esprintando al rango");
            stalkerReference.PlayerIsInRange();
            stalkerReference.HaltCoroutine();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        Player script = other.GetComponent<Player>();

        bool hasSprinted = false;

        if (script != null && !hasSprinted)
        {
            if (script.isSprinting)
            {
                Debug.Log("El player esprintó dentro del rango");
                stalkerReference.PlayerIsInRange();
                stalkerReference.HaltCoroutine();

                hasSprinted = true;
            }
        }
    }
}
