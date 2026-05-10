using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected int life;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;
    public int CurrentLife => life;

    public virtual void GetDamage(int d)
    {
        life -= d;
        Debug.Log($"{gameObject.name} recibe {d} de daño. Vida restante: {life}");

        if (life <= 0)
        {
            Death();
        }
    }

    public abstract void Death();
}
