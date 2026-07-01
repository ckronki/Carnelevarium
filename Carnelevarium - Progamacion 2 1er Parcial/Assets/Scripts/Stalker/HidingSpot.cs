using UnityEngine;

public class HidingSpot : MonoBehaviour
{
    [SerializeField] int thisSpotIndex;
    private void Start()
    {
        for (int i = 0; i < GameManager.instance.stalkerMovement.hidingSpots.Length; i++)
        {
            if (GameManager.instance.stalkerMovement.hidingSpots[i] == this.transform)
            {
                Debug.Log("Índice del hiding spot " + i + " encontrado");
                    
                thisSpotIndex = i;
                return;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        StalkerMovement script = other.GetComponent<StalkerMovement>();

        if (script != null && script.canHide)
        {
            int hidingSpotIndex = Random.Range(0, script.hidingSpots.Length);
            
            script.HidingState();

            script.PathResetState();

            script.Teleport(script.hidingSpots[hidingSpotIndex]);

            script.PathResetState();
        }


    }


}
