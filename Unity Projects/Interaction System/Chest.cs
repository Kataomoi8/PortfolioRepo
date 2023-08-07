using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;

    public bool Interact(Interactor interactor)
    {
        Debug.Log("Opening menu");
        return true;
    }
}
