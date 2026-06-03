using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState { Idle, ShowSequence, PlayerInput, Win, Lose }

public class SimonSaysManager : MonoBehaviour
{
    [Header("Configuración")]
    public int maxRounds = 10;
    public float showDelay = 0.6f;
    public float flashDuration = 0.4f;

    [Header("Referencias")]
    public SimonButton[] buttons;
    public SimonSoundsManager sounds;
    public SimonSaysUI ui;
    public InteractionController interactionController;

    private List<int> sequence = new List<int>();
    private int playerIndex = 0;
    private GameState state = GameState.Idle;

    void Update()
    {
        if (state == GameState.Idle) return; // 👈 si el panel está cerrado, no hace nada

        if (Input.GetKeyDown(KeyCode.Escape) && state != GameState.ShowSequence)
        {
            StopAllCoroutines();
            ClosePanel();
        }
    }

    public void StartGame()
    {
        sequence.Clear();
        playerIndex = 0;
        state = GameState.Idle;

        interactionController.LockInteraction();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        ui.ShowPanel(true);
        ui.SetMessage("¡Memoriza la secuencia!");
        ui.SetScore(0);
        NextRound();
    }

    void NextRound()
    {
        playerIndex = 0;
        sequence.Add(Random.Range(0, buttons.Length));
        ui.SetScore(sequence.Count - 1);
        StartCoroutine(ShowSequence());
    }

    IEnumerator ShowSequence()
    {
        state = GameState.ShowSequence;
        SetButtonsInteractable(false);
        ui.SetMessage("Observa...");

        yield return new WaitForSeconds(0.5f);

        foreach (int index in sequence)
        {
            yield return StartCoroutine(buttons[index].Flash(flashDuration));
            sounds.PlayTone(index);
            yield return new WaitForSeconds(showDelay);
        }

        state = GameState.PlayerInput;
        SetButtonsInteractable(true);
        ui.SetMessage("¡Tu turno!");
    }

    public void OnButtonPressed(int index)
    {
        if (state != GameState.PlayerInput) return;

        sounds.PlayTone(index);

        if (index == sequence[playerIndex])
        {
            playerIndex++;

            if (playerIndex >= sequence.Count)
            {
                if (sequence.Count >= maxRounds)
                    StartCoroutine(WinGame());
                else
                {
                    SetButtonsInteractable(false);
                    Invoke(nameof(NextRound), 1f);
                }
            }
        }
        else
        {
            StartCoroutine(LoseGame(index));
        }
    }

    IEnumerator WinGame()
    {
        state = GameState.Win;
        SetButtonsInteractable(false);
        sounds.PlayWin();
        ui.SetMessage("🎉 ¡Ganaste!");
        yield return new WaitForSeconds(2f);
        ClosePanel();
    }

    IEnumerator LoseGame(int wrongIndex)
    {
        state = GameState.Lose;
        SetButtonsInteractable(false);
        yield return StartCoroutine(buttons[wrongIndex].FlashError());
        sounds.PlayLose();
        ui.SetMessage($"❌ Era el {sequence[playerIndex] + 1}. Ronda {sequence.Count}");
        yield return new WaitForSeconds(2.5f);
        ClosePanel();
    }

    void ClosePanel()
    {
        state = GameState.Idle;
        SetButtonsInteractable(false);
        interactionController.UnlockInteraction();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        ui.ShowPanel(false);
    }

    void SetButtonsInteractable(bool value)
    {
        foreach (var btn in buttons)
            btn.SetInteractable(value);
    }
}