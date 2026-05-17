using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Stalker : Entity
{
    public Transform objetive;
    private NavMeshAgent _stalker;

    [SerializeField] Animator animator;

    [SerializeField] Door controlledDoor;


    void Start()
    {
        _stalker = GetComponent<NavMeshAgent>();
        _stalker.stoppingDistance = 1.0f; // se detiene a 1 unidad del Player
    }

    void Update()
    {
        if (controlledDoor != null && controlledDoor.isOpen)
        {
            _stalker.speed = speed;

            if (objetive != null)
            {
                _stalker.SetDestination(objetive.position);
            }
        }
        else
        {
            // Quieto si la puerta no está abierta
            _stalker.SetDestination(transform.position);
            _stalker.speed = 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.GetComponent<Health>())
        {
            Player player = other.GetComponent<Player>();
            player.GetDamage(damage); // usa el damage heredado de Entity
            animator.SetBool("Attack", true);
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        animator.SetBool("Attack", false);
    }
    public override void Death()
    {
      //stunn
    }

}