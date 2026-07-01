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

    [SerializeField] private float attackCooldown = 1.5f; // tiempo entre ataques
    private float nextAttackTime = 0f;

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

            animator.SetBool("CanMove", true);

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
        if (other.GetComponent<Player>().CurrentLife > 0)
        {
            AttackPlayer();
            animator.SetBool("Attack", true);
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Player>().CurrentLife>0)
        {
            AttackPlayer();
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

    private void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            Player p = objetive.GetComponent<Player>();
            if (p != null)
            {
                p.GetDamage(damage);
                Debug.Log($"{gameObject.name} ataca al jugador. Daño: {damage}");
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }

}