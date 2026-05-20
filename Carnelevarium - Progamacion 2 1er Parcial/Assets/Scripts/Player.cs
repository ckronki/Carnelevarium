using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public InputActionReference controlMove;
    [SerializeField] private Rigidbody rb;

    public Animator animator;
    Coroutine _currentCoroutine;
    float _backupSpeed;

    public AudioSource audioSource;
    public AudioClip footSteps;

    private void Awake()
    {
        _backupSpeed = speed;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Vector2 move = controlMove.action.ReadValue<Vector2>();//Input.GetAxis

        Vector3 dir = transform.forward * move.y;
        dir += transform.right * move.x;

        rb.linearVelocity = dir * speed;

        if (move.magnitude != 0)
        {
            animator.SetBool("isWalking", true);
        }
        else
            animator.SetBool("isWalking", false);


        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (move != Vector2.zero)
        {
            Walking();
        }
        else
        {
            audioSource.Stop();
        }
    }

    public void Walking()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footSteps;   // asigno el clip
            audioSource.loop = true;        // que se repita mientras camina
            audioSource.Play();             // empieza a sonar
        }
        
    }  
    


    

    public override void Death()
    {
        Invoke(nameof(LoadDeathScene), 1.5f);
    }

    private void LoadDeathScene()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene(2);
    }
    public override void GetDamage(int d)
    {
        base.GetDamage(d);
        Debug.Log("El jugador recibe " + d + " de daño. Vida restante: " + life);
    }
    public void Freeze(float duration)
    {
        if (_currentCoroutine != null)
            StopCoroutine(_currentCoroutine);

        _currentCoroutine = StartCoroutine(FreezeRoutine(duration));
    }

    private System.Collections.IEnumerator FreezeRoutine(float duration)
    {
        float originalSpeed = _backupSpeed;
        speed = 0f;
        yield return new WaitForSeconds(duration);
        speed = originalSpeed;
    }

    public void ChangeSpeed(float multiplier)
    {
        speed *= multiplier;
        _backupSpeed = speed;
    }

    public void ResetSpeed(float originalValue)
    {
        speed = originalValue;
        _backupSpeed = speed;
    }


    public float CurrentSpeed => speed;


    public void AttackAnimation()
    {
        animator.SetBool("IsAttacking", true);
        Debug.Log("Se reproduce la animación de ataque");
    }

    


}