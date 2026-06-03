using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class SimonSaysOpen : Interactables, IInteractable
{
    [SerializeField] private SimonSaysManager simonManager;

    public void Interact()
    {
        simonManager.StartGame();
    }

    void OnMouseDown()
    {
        simonManager.StartGame();
    }
}
