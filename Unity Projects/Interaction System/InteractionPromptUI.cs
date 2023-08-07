using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [SerializeField] private GameObject uiPanel;
    [SerializeField] private TextMeshProUGUI promptText;

    private GameManager manager;

    private void Start()
    {
        uiPanel.SetActive(false);
        manager = FindObjectOfType<GameManager>();
    }

    private void LateUpdate()
    {
        if (manager.GetState() == GameManager.GameState.Menu)
        {
            Close();
        }
    }

    public bool isDisplayed = false;
    public void SetUp(string text)
    {
        promptText.text = text;
        uiPanel.SetActive(true);
        isDisplayed = true;
    }

    public void Close()
    {
        isDisplayed = false;
        uiPanel.SetActive(false);
    }
}
