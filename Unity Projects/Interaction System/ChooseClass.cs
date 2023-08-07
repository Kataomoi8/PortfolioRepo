using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChooseClass : MonoBehaviour, IInteractable
{
    [SerializeField] private string prompt;

    public string InteractionPrompt => prompt;

    [SerializeField] private GameObject chooseClassUI;

    private GameManager manager;

    public bool Interact(Interactor interactor)
    {
        //This is where we will open the menu to pick a class
        manager.SetState(GameManager.GameState.Menu);
        chooseClassUI.SetActive(true);
        return true;
    }

    private void Start()
    {
        manager = FindObjectOfType<GameManager>();
        chooseClassUI.SetActive(false);
    }

    public void CloseUI()
    {
        manager.SetState(GameManager.GameState.Running);
        chooseClassUI.SetActive(false);
    }
}
