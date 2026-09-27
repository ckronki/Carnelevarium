using UnityEngine;
using System;

namespace Game.Enemies // TP2 Ludmila perez arias - namespace, get/set, evento sonido de muerte
{
    public class EnemyBasic : Enemy
    {
        [SerializeField] private float attackCooldown = 1.5f;
        private float nextAttackTime = 0f;

        [SerializeField] Animator animator;

        //getter/setter para vida
        public int Life
        {
            get => life;
            private set => life = Mathf.Max(0, value);
        }

        

        public void Start()
        {
            animator = GetComponent<Animator>();
            
        }

        protected override void ChasePlayer(float distance)
        {
            if (detectionRange <= 0) return;

            if (distance > attackRange)
            {
                transform.LookAt(player);
                transform.position = Vector3.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
                Debug.Log($"{gameObject.name} persigue al jugador");

                animator.SetBool("IsMoving", true);
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

                animator.SetTrigger("IsAttacking");

                nextAttackTime = Time.time + attackCooldown;
            }
        }

        public override void GetDamage(int d)
        {
            if (Life <= 0) return; //ya murio, mientras se disuelve no recibe mas golpes

            Life -= d; //para q no quede en numeros negativos
            Debug.Log($"{gameObject.name} recibe {d} de daño. Vida restante: {Life}");

            if (Life <= 0)
            {
                Death();
                return; //si murio no hace la animacion de golpe
            }

            animator.SetTrigger("WasHit");
        }

        //evento al morir
        public event Action<EnemyBasic> OnEnemyDeath;
        public override void Death()
        {
            //si tiene el efecto de disolverse, deja de perseguir y se disuelve (se destruye solo al terminar)
            DissolvingControllerTut dissolve = GetComponent<DissolvingControllerTut>();
            if (dissolve != null)
            {
                enabled = false;
                dissolve.StartDissolve();
            }
            else
            {
                gameObject.SetActive(false);
            }

            //sonido al morir
            AudioSource audio = GetComponent<AudioSource>();
            if (audio != null)
            {
                audio.Play(); //audioSource
            }

            //evento de muerte
            OnEnemyDeath?.Invoke(this);
        }
    }
}