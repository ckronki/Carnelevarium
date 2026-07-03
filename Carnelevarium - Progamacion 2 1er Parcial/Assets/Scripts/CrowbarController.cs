using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrowbarController : MonoBehaviour
{
    public GameObject crowbar;
    public bool canAttack = true;
    public float attackCooldown;
    public bool isAttacking;

    [SerializeField] Camera _playerCamera;

    public float attackDistance;
    public float attackDelay;
    public float attackSpeed;
    public int attackDamage;
    public LayerMask attackLayer;

    public void Update()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (!canAttack || isAttacking) return;

        Debug.Log("El player atacó");

        canAttack = false;
        isAttacking = true;

        Invoke(nameof(AttackRaycast), attackDelay);

        StartCoroutine(ResetAttackCooldown());
        
        AttackAnimation();
    }

    public void AttackRaycast()
    {
        var ray = _playerCamera.ViewportPointToRay(new Vector2(0.5f, 0.5f));
        
        if (Physics.Raycast(ray, out RaycastHit hit, attackDistance, attackLayer))
        {
            hit.collider.GetComponent<Entity>().GetDamage(attackDamage);
            Debug.Log("El player golpeó a " +  hit.collider.name);
            Debug.Log("La vida de " + hit.collider.name + " es: " + hit.collider.GetComponent<Entity>().CurrentLife);
        }
    }

    public void AttackAnimation()
    {
        Animator animator = crowbar.GetComponent<Animator>();
        animator.SetTrigger("Attack");
    }

    public IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(attackSpeed);

        canAttack = true;
        isAttacking = false;
    }

    public void AttackLock()
    {
        canAttack = false;
        this.gameObject.SetActive(false);
    }

    public void AttackUnlock()
    {
        canAttack = true;
        this.gameObject.SetActive(true);
    }
}
