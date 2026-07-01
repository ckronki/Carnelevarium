using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class StalkerMovement : Enemy
{
    private NavMeshAgent navMeshAgent;

    [SerializeField] Transform[] waypoints;
    [SerializeField] public Transform[] hidingSpots;
    [SerializeField] Animator animator;

    public Transform currentWaypoint;

    [SerializeField] float pathUpdateDelay;
    private float pathUpdateDeadline;

    public float timeTillPathReset;

    public bool isPlayerInRange = false;
    public bool isResettingPath = false;
    public bool canHide = false;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.stoppingDistance = attackRange;

        detectionRange = 0; //se hardcodea el detectionrange a 0 para detectar al jugador usando colliders con triggers
    }

    protected void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (isPlayerInRange)
        {
            ChasePlayer(distance);

            animator.SetBool("IsPlayerInRange", true);
        }
        else
        {
            if (isResettingPath)
            {
                if (GameManager.instance.player.isPlayerInSafeRoom && canHide)
                {
                    ChooseClosestWaypoint(hidingSpots);
                }
                else if (!canHide)
                {
                    ChooseClosestWaypoint(waypoints);
                }
            }
            else if (!isResettingPath)
            {
                UpdatePath(currentWaypoint);

                animator.SetBool("IsPlayerInRange", false);
            }
        }
    }

    public void Teleport(Transform tpSpot)
    {
        transform.position = tpSpot.position;
        canHide = false;
    }

    protected override void ChasePlayer(float distance)
    {
        NavMeshPath path = new NavMeshPath();

        navMeshAgent.CalculatePath(player.position, path);

        Debug.Log("Estado del path: " + path.status);

        if (path.status == NavMeshPathStatus.PathComplete)
        {
            if (distance > attackRange)
            {
                transform.LookAt(player);

                UpdatePath(player);

                Debug.Log($"{gameObject.name} persigue al jugador");
            }
            else
            {
                AttackPlayer();
            }
        }
        else
        {
            Debug.Log("El player no puede ser alcanzado");
            isPlayerInRange = false;
            isResettingPath = true;
            canHide = true;
            HaltCoroutine();
        }
    }

    protected override void AttackPlayer()
    {
        animator.SetTrigger("IsAttacking");
    }

    #region OnTriggers
    public void OnTriggerEnter(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && !script.isPlayerInSafeRoom)
        {
            Debug.Log("El player entró al rango");
            isPlayerInRange = true;
            HaltCoroutine();
        }
    }

    public void OnTriggerStay(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && !script.isPlayerInSafeRoom)
        {
            Debug.Log("El player se mantiene en el rango");
            isPlayerInRange = true;
            HaltCoroutine();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        Player script = other.GetComponent<Player>();

        if (script != null && !script.isPlayerInSafeRoom)
        {
            Debug.Log("El player salió de rango");
            StartCoroutine(VerifyPlayerInRange(timeTillPathReset));
        }
    }
    #endregion

    public IEnumerator VerifyPlayerInRange(float s)
    {
        Debug.Log("La corutina empezó");

        yield return new WaitForSeconds(s);

        isPlayerInRange = false;

        isResettingPath = true;

        Debug.Log("La corutina terminó");
    }

    private void UpdatePath(Transform t)
    {
        if (Time.time >= pathUpdateDeadline)
        {
            Debug.Log("Actualizando camino");
            pathUpdateDeadline = Time.time + pathUpdateDelay;
            navMeshAgent.SetDestination(t.position);
        }
    }

    public void ChooseClosestWaypoint(Transform[] tArray)
    {
        float closestTargetDistance = float.MaxValue;
        NavMeshPath path = null;
        NavMeshPath shortestPath = null;

        for (int i = 0; i < tArray.Length; i++)
        {
            if (tArray[i] == null)
                continue;

            path = new NavMeshPath();

            if (NavMesh.CalculatePath(transform.position, tArray[i].position, navMeshAgent.areaMask, path))
            {
                float distance = Vector3.Distance(transform.position, path.corners[0]);

                for (int j = 1; j < path.corners.Length; j++)
                {
                    distance += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }

                if (distance < closestTargetDistance)
                {
                    closestTargetDistance = distance;
                    shortestPath = path;
                }
            }
        }

        if (shortestPath != null)
        {
            navMeshAgent.SetPath(shortestPath);
        }
    }

    public void SetWaypoint(Transform t)
    {
        currentWaypoint = t;
    }

    public void HidingState()
    {
        if (!canHide)
        {
            canHide = true;
        }
        else
        {
            canHide = false;
        }
    }

    public void PlayerIsInRange()
    {
        isPlayerInRange = true;
    }

    public void PathResetState()
    {
        if (!isResettingPath)
        {
            isResettingPath = true;
        }
        else
        {
            isResettingPath = false;
        }
    }

    public void HaltCoroutine()
    {
        Debug.Log("Corutina detenida");
        StopCoroutine(VerifyPlayerInRange(timeTillPathReset));
    }

    #region Métodos requeridos obligatoriamente para buildear el script
    public override void Death()
    {

    }
    #endregion

}
