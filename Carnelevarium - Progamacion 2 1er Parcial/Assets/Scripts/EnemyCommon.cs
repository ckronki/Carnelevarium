using UnityEngine;

public class EnemyBasic : Enemy
{
    [SerializeField] private float attackCooldown = 1.5f; // tiempo entre ataques
    private float nextAttackTime = 0f;

    private void Awake()
    {
        life = 80;
        damage = 20;
        speed = 3f;
        detectionRange = 10f;
        attackRange = 2f;
    }

    protected override void ChasePlayer(float distance)
    {
        // detectionRange es 0, no hace nada
        if (detectionRange <= 0) return;

        if (distance > attackRange)
        {
            transform.LookAt(player);
            transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            Debug.Log($"{gameObject.name} persigue al jugador");
        }
        else
        {
            AttackPlayer();
        }
    }

    protected override void AttackPlayer()
    {
        if (Time.time >= nextAttackTime)
        {
            Player p = player.GetComponent<Player>();
            if (p != null)
            {
                p.GetDamage(damage);
                Debug.Log($"{gameObject.name} ataca al jugador. Daño: {damage}");
            }

            nextAttackTime = Time.time + attackCooldown;
        }
    }
    public override void Death()
    {
        Destroy(gameObject);
    }
}