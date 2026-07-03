using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Audio;
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
    public bool canMove = false;

    [SerializeField] float attackCooldown;
    private float nextAttack;

    [SerializeField] AudioSource frontSteps;
    [SerializeField] AudioSource backSteps;
    [SerializeField] AudioClip[] footSteps;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        navMeshAgent.stoppingDistance = detectionRange;
    }

    protected void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (canMove)
        {
            frontSteps.enabled = true;
            backSteps.enabled = true;

            if (isPlayerInRange)
            {
                if (distance > attackRange)
                {
                    ChasePlayer(distance);

                    animator.SetBool("IsPlayerInRange", true);
                }
                else
                {
                    AttackPlayer();
                }

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
        else
        {
            frontSteps.enabled = false;
            backSteps.enabled = false;
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
            transform.LookAt(player);

            UpdatePath(player);

            Debug.Log($"{gameObject.name} persigue al jugador");
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
        if (Time.time > nextAttack)
        {
            nextAttack = Time.time + attackCooldown;

            Player p = GameManager.instance.player;

            if (p != null)
            {
                p.GetDamage(damage);
                Debug.Log($"{gameObject.name} ataca al jugador. Daño: {damage}");

                animator.SetTrigger("IsAttacking");
                Debug.Log("El ataque se realizó");
            }

        }
        else
        {
            Debug.Log("El ataque se encuentra en cooldown");
        }
    }

    public void PlayFrontFootStep()
    {
        int footStepsIndex = Random.Range(0, footSteps.Length);

        Debug.Log("El stalker ha elegido el paso " + footStepsIndex);

        frontSteps.clip = footSteps[footStepsIndex];
        frontSteps.Play();
    }

    public void PlayBackFootStep()
    {
        int footStepsIndex = Random.Range(0, footSteps.Length);

        Debug.Log("El stalker ha elegido el paso " + footStepsIndex);

        backSteps.clip = footSteps[footStepsIndex];
        backSteps.Play();
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

    //private void UpdatePath(Transform t)
    //{
    //    Debug.Log("Actualizando camino");
    //    pathUpdateDeadline = Time.time + pathUpdateDelay;

    //    if (Time.time >= pathUpdateDeadline)
    //    {
    //        //NavMeshPath path = new NavMeshPath();

    //        //navMeshAgent.CalculatePath(t.position, path);

    //        //Debug.Log("Estado del path: " + path.status);

    //        //if (path.status == NavMeshPathStatus.PathComplete)
    //        //{
    //            navMeshAgent.SetDestination(t.position);
    //        //}
    //        //else
    //        //{
    //        //    Debug.Log("El waypoint elegido no puede ser alcanzado");

    //        //    ChooseClosestWaypoint(hidingSpots);

    //        //    Debug.Log("El stalker ha elegido un waypoint nuevo");
    //        //}
    //    }
    //}

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

    public void MovementState()
    {
        canMove = true;
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
