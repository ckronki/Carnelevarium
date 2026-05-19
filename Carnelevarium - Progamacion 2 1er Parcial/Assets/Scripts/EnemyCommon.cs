using UnityEngine;

public class EnemyBasic : Enemy
{
    [SerializeField] private float attackCooldown = 1.5f; // tiempo entre ataques
    private float nextAttackTime = 0f;

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
        gameObject.SetActive(false); // En vez de Destroy
    }
}