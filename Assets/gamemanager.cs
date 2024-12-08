using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Image goodsarpanch, badsarpanch, secretary, minister;
    public Slider corruptionMeter, moraleMeter, politicalStandingMeter;
    public TextMeshProUGUI dialogueText;
    public GameObject choicePanel;
    public Button[] choiceButtons;

    public Button prevButton; // Button to go to the previous dialogue
    public Button nextButton; // Button to go to the next dialogue manually

    private int currentDialogueIndex = 0;
    private int ministerChoice = -1; // -1 means no choice yet

    [System.Serializable]
    public struct Dialogue
    {
        public string character;
        public string text;
        public bool hasChoices;
        public string[] choices;
    }

    public Dialogue[] dialogues;

    void Start()
    {
        choicePanel.SetActive(false);
        prevButton.onClick.AddListener(ShowPreviousDialogue);
        nextButton.onClick.AddListener(ShowNextDialogue);

        UpdateNavigationButtons();
        ShowNextDialogue(); // Start with the first dialogue
    }

    void Update()
    {
        // Right-click for previous dialogue
        if (Input.GetMouseButtonDown(1)) // Right mouse button (index 1)
        {
            ShowPreviousDialogue();
        }

        // Left-click for next dialogue
        if (Input.GetMouseButtonDown(0)) // Left mouse button (index 0)
        {
            ShowNextDialogue();
        }
    }

    void UpdateNavigationButtons()
    {
        // Disable previous button if on the first dialogue
        prevButton.interactable = currentDialogueIndex > 0;

        // Disable next button if on the last dialogue and no choices are active
        nextButton.interactable = currentDialogueIndex < dialogues.Length - 1 && !choicePanel.activeSelf;
    }

    public void ShowNextDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length) return;

        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        // Reset character visibility
        goodsarpanch.enabled = false;
        badsarpanch.enabled = false;
        secretary.enabled = false;
        minister.enabled = false;

        // Handle choices
        if (currentDialogue.hasChoices)
        {
            dialogueText.text = ""; // No text shown before the choice
            ShowChoices(currentDialogue.choices);
            UpdateNavigationButtons();
            return;
        }

        // Set dialogue text and character visibility if no choices
        dialogueText.text = currentDialogue.text;

        // Trigger camera shake and character visibility
        switch (currentDialogue.character)
        {
            case "goodsarpanch":
                goodsarpanch.enabled = true;
                break;
            case "badsarpanch":
                badsarpanch.enabled = true;
                break;
            case "secretary":
                secretary.enabled = true;
                break;
            case "minister":
                minister.enabled = true;
                break;
        }

        currentDialogueIndex++;
        UpdateNavigationButtons();
    }

    public void ShowPreviousDialogue()
    {
        if (currentDialogueIndex <= 0) return;

        currentDialogueIndex--;

        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        // Reset character visibility
        goodsarpanch.enabled = false;
        badsarpanch.enabled = false;
        secretary.enabled = false;
        minister.enabled = false;

        // Set dialogue text and character visibility
        dialogueText.text = currentDialogue.text;

        // Trigger camera shake and character visibility
        switch (currentDialogue.character)
        {
            case "goodsarpanch":
                goodsarpanch.enabled = true;
                break;
            case "badsarpanch":
                badsarpanch.enabled = true;
                break;
            case "secretary":
                secretary.enabled = true;
                break;
            case "minister":
                minister.enabled = true;
                break;
        }

        UpdateNavigationButtons();
    }

    private void ShowChoices(string[] choices)
    {
        choicePanel.SetActive(true);
        minister.enabled = true;

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => HandleChoice(index));
        }

        // Deactivate unused buttons
        for (int i = choices.Length; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void HandleChoice(int choiceIndex)
    {
        ministerChoice = choiceIndex; // Capture minister's choice
        choicePanel.SetActive(false);

        // Display the selected choice as dialogue text
        dialogueText.text = dialogues[currentDialogueIndex].choices[choiceIndex];

        // Adjust sliders based on choice
        switch (choiceIndex)
        {
            case 0: // Good choice
                corruptionMeter.value = Mathf.Max(0, corruptionMeter.value - 8);
                moraleMeter.value = Mathf.Min(100, moraleMeter.value + 8);
                politicalStandingMeter.value = Mathf.Min(100, politicalStandingMeter.value + 3);
                break;
            case 1: // Neutral choice
                corruptionMeter.value = Mathf.Min(100, corruptionMeter.value + 3);
                moraleMeter.value = Mathf.Max(0, moraleMeter.value - 2);
                break;
            case 2: // Bad choice
                corruptionMeter.value = Mathf.Min(100, corruptionMeter.value + 8);
                moraleMeter.value = Mathf.Max(0, moraleMeter.value - 8);
                politicalStandingMeter.value = Mathf.Max(0, politicalStandingMeter.value - 3);
                break;
        }

        currentDialogueIndex++;
        UpdateNavigationButtons();
        ShowNextDialogue();
    }
}