using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Generator : ItemCheck , IInteractable
{
    [TextArea][SerializeField] string crowbarntDialogue;
    [TextArea][SerializeField] string fusentDialogue;
    [TextArea][SerializeField] string valventDialogue;

    [TextArea][SerializeField] string crowbarDialogue;
    [TextArea][SerializeField] string fuseDialogue;

    [TextArea][SerializeField] string winDialogue;

    [SerializeField] float dialogueTime;

    [SerializeField] Transform macarenaCam;
    [SerializeField] Camera playerCamera;

    public bool hasUsedCrowbar;
    public bool hasUsedFuse;
    public bool hasUsedValve;

    [Header("Referencias")]
    public Animator crowbarAnimator;
    public InteractionController interactionController;

    [Header("Configuraci�n")]
    public float animationDuration = 3f;

    private bool hasBeenFixed = false;


    [Header("Luces")]
    public Light luzCrowbar;  // se pone verde al usar la crowbar
    public Light luzFuse;     // se pone verde al usar el fusible
    public Light luzValve;    // se pone verde al usar la v�lvula

    public Color colorApagado = Color.red;
    public Color colorEncendido = Color.green;

    void Start()
    {
        luzCrowbar.color = colorApagado;
        luzFuse.color = colorApagado;
        luzValve.color = colorApagado;
    }

    public void Update()
    {
        if (hasUsedValve == true)
        {
            GameManager.instance.crowbarController.AttackLock();
            GameManager.instance.player.CantMove();
            GameManager.instance.cameraController.LockCamera();

            playerCamera.transform.position = macarenaCam.position;
            playerCamera.transform.rotation = macarenaCam.rotation;

            UIManager.instance.dialogueText.text = winDialogue;
        }
    }

    public void Interact()
    {
        InventoryCheck();

        if (hasFoundItem)
        {
            if (!hasUsedCrowbar)
            {
                //StartCoroutine(HasInteracted(crowbarDialogue, dialogueTime));
                StartCoroutine(CrowbarAnimation()); // arranca la animaci�n
                hasUsedCrowbar = true;
                hasFoundItem = false;

            }
            else if (hasUsedCrowbar && !hasUsedFuse)
            {
                StartCoroutine(HasInteracted(fuseDialogue, dialogueTime));
                hasUsedFuse = true;
                hasFoundItem = false;
                luzFuse.color = colorEncendido;    // segunda luz verde
            }
            else if (hasUsedCrowbar && hasUsedFuse && !hasUsedValve)
            {
                hasUsedValve = true;
                luzValve.color = colorEncendido;   // tercera luz verde
            }
        }
        else if (!hasFoundItem)
        {
            if (!hasUsedCrowbar)
            {
                StartCoroutine(HasInteracted(crowbarntDialogue, dialogueTime));
                return;
            }
            else if (hasUsedCrowbar && !hasUsedFuse)
            {
                StartCoroutine(HasInteracted(fusentDialogue, dialogueTime));
            }
            else if (hasUsedCrowbar && hasUsedFuse && !hasUsedValve)
            {
                StartCoroutine(HasInteracted(valventDialogue, dialogueTime));
            }
        }
    }

    public IEnumerator HasInteracted(string dialogue, float time)
    {
        UIManager.instance.dialogueText.text = dialogue;

        yield return new WaitForSeconds(time);

        UIManager.instance.dialogueText.text = "";
    }

    IEnumerator CrowbarAnimation()
    {
        // 1. Bloquear al jugador
        interactionController.LockInteraction();

        // 2. Mostrar di�logo
        UIManager.instance.dialogueText.text = crowbarDialogue;

        // 3. Reproducir animaci�n de la crowbar
        crowbarAnimator.SetTrigger("Fix");


        // 4. Esperar a que termine la animaci�n
        yield return new WaitForSeconds(animationDuration);

        // 5. Limpiar di�logo y desbloquear jugador
        UIManager.instance.dialogueText.text = "";
        interactionController.UnlockInteraction();

        // 6. Cambiar el color de la luz a verde
        luzCrowbar.color = colorEncendido;
    }
}
