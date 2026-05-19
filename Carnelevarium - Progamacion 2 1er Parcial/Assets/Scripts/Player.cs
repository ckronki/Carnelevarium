using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
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
        //Vector3 dir = new Vector3(move.x, 0, move.y);
        transform.position += dir * speed * Time.deltaTime;

        if (move.magnitude != 0)
        {
            animator.SetBool("isWalking", true);
        }   
        else
            animator.SetBool("isWalking", false);


        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsName("Walking") && !audioSource.isPlaying)
        {
            Walking();
        }


        

    }

    public void Walking ()
    {
     if (!audioSource.isPlaying)
     {
        audioSource.clip = footSteps;   // asigno el clip
        audioSource.loop = true;        // que se repita mientras camina
        audioSource.Play();             // empieza a sonar
     }

       else
       {
          if (audioSource.isPlaying)
          {
            audioSource.Stop();             // se detiene al dejar de caminar
          }
       }  
    }


    public override void Death()
    {
        Invoke(nameof(LoadDeathScene), 3f);
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

}