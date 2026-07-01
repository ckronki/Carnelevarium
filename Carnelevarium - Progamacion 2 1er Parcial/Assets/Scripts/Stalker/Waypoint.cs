using UnityEngine;

public class Waypoint : MonoBehaviour
{
    [SerializeField] Transform[] waypoints;

    private void OnTriggerEnter(Collider other)
    {
        int waypointIndex = Random.Range(0, waypoints.Length);
        
        StalkerMovement script = other.GetComponent<StalkerMovement>();

        if (script != null && !script.canHide)
        {
            Debug.Log("El stalker llegó al " + name);
            
            script.SetWaypoint(waypoints[waypointIndex]);

            if (script.isResettingPath)
            {
                script.PathResetState();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        StalkerMovement script = other.GetComponent<StalkerMovement>();

        if (script != null && script.isResettingPath)
        {
            if (!script.canHide)
            {
                script.PathResetState();
            }
        }
    }
}