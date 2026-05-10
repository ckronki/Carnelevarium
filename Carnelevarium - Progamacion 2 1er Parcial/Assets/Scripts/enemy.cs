using UnityEngine;

public abstract class Enemy : Entity
{
    [SerializeField] protected float detectionRange = 10f;
    [SerializeField] protected float attackRange = 2f;
    [SerializeField] protected Transform player; 

    protected virtual void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        
        if (detectionRange > 0 && distance <= detectionRange)
        {
            ChasePlayer(distance);
        }
    }

    protected abstract void ChasePlayer(float distance);
    protected abstract void AttackPlayer();
}