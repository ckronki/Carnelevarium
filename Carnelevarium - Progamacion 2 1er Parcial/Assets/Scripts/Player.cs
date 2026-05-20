using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.Collections;

public class Player : Entity
{
    public InputActionReference controlMove;
    public InputActionReference sprintAction; 
    [SerializeField] private Rigidbody rb;

    public Animator animator;
    Coroutine _currentCoroutine;
    float _backupSpeed;

    public AudioSource audioSource;
    public AudioClip footSteps;

    public bool cantMove;
    // --- Sprint & Stamina ---
    [Header("Sprint Settings")]
    [SerializeField] float sprintMultiplier; 
    [SerializeField] public float staminaMax;        
    [SerializeField] float staminaMin;         
    [SerializeField] float staminaRegenRate;   

    public float staminaCurrent;
    private bool isSprinting;
    private bool exhausted;

    public bool hasCrowbar;

    private void Awake()
    {
        _backupSpeed = speed;
        staminaCurrent = staminaMax;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (cantMove == true) return;
        else 
        {
            Vector2 move = controlMove.action.ReadValue<Vector2>();

            Vector3 dir = transform.forward * move.y;
            dir += transform.right * move.x;
            
            rb.linearVelocity = dir * speed;

            HandleSprint(move);

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
        
    }
    public void setStun()
    {
        cantMove = true;
        rb.linearVelocity = Vector3.zero;
    }

    public void CantMove()
    {
        cantMove = true;
    }

    public void CanMove()
    {
        cantMove = false;
    }

    public void GetCrowbar()
    {
        hasCrowbar = true;
    }

    public void EnableSprint()
    {
        exhausted = false;
    }
    public void ForceStopSprint()
    {
        if (isSprinting)
        {
            speed = _backupSpeed;
            isSprinting = false;
        }
    }
    public void DisableSprintTemporarily(float duration)
    {
        StartCoroutine(DisableSprintRoutine(duration));
    }

    private IEnumerator DisableSprintRoutine(float duration)
    {
        exhausted = true; 
        yield return new WaitForSeconds(duration);
        exhausted = false; 
    }
    private void HandleSprint(Vector2 move)
    {
        bool sprintPressed = sprintAction.action.IsPressed();

        if (sprintPressed && move.magnitude > 0 && !exhausted)
        {
            if (!isSprinting && staminaCurrent > staminaMin)
            {
                speed = _backupSpeed * sprintMultiplier;
                isSprinting = true;
            }

            if (isSprinting)
            {
                staminaCurrent -= Time.deltaTime;
                if (staminaCurrent <= 0f)
                {
                    staminaCurrent = 0f;
                    StopSprint();
                    exhausted = true;
                }
            }
        }
        else
        {
            StopSprint();
        }

        // Regeneración de stamina
        if (!isSprinting)
        {
            staminaCurrent += staminaRegenRate * Time.deltaTime;
            staminaCurrent = Mathf.Clamp(staminaCurrent, 0f, staminaMax);

            if (exhausted && staminaCurrent >= staminaMax)
            {
                exhausted = false; // ya puede volver a sprintar
            }
        }
    }
    private void StopSprint()
    {
        if (isSprinting)
        {
            speed = _backupSpeed;
            isSprinting = false;
        }
    }

    public void Walking()
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = footSteps;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public override void Death()
    {
        Invoke(nameof(LoadDeathScene), 2f);
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
        cantMove = false;
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
