using UnityEngine;

public abstract class Entity : MonoBehaviour //TP2 ludmila perez arias, get/set, encapsulamiento
{
    [SerializeField] protected int life;
    [SerializeField] protected int damage;
    [SerializeField] protected float speed;

    //para acceder vida con control
    public int Life
    {
        get => life;
        set => life = Mathf.Max(0, value); 
    }

    //consultar la vida actual
    public int CurrentLife => life;

    public virtual void GetDamage(int d)
    {
        Life -= d; //usa la propiedad  en vez de directamente
        Debug.Log($"{gameObject.name} recibe {d} de daño. Vida restante: {Life}");

        if (Life <= 0)
        {
            Death();
        }
    }

    public abstract void Death();
}
