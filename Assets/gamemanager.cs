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
        ShowNextDialogue();
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

        // Set dialogue text and character visibility
        dialogueText.text = currentDialogue.text;

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
                if (currentDialogue.hasChoices)
                {
                    ShowChoices(currentDialogue.choices);
                    return; // Wait for player's choice
                }
                break;
        }

        currentDialogueIndex++;
    }

    private void ShowChoices(string[] choices)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => HandleChoice(index));
        }
    }

    private void HandleChoice(int choiceIndex)
    {
        ministerChoice = choiceIndex; // Capture minister's choice
        choicePanel.SetActive(false);

        // Adjust sliders based on choice
        switch (choiceIndex)
        {
            case 0: // Good choice
                corruptionMeter.value = Mathf.Max(0, corruptionMeter.value - 10);
                moraleMeter.value = Mathf.Min(100, moraleMeter.value + 10);
                politicalStandingMeter.value = Mathf.Min(100, politicalStandingMeter.value + 5);
                break;
            case 1: // Neutral choice
                // Neutral changes
                break;
            case 2: // Bad choice
                corruptionMeter.value = Mathf.Min(100, corruptionMeter.value + 10);
                moraleMeter.value = Mathf.Max(0, moraleMeter.value - 10);
                politicalStandingMeter.value = Mathf.Max(0, politicalStandingMeter.value - 5);
                break;
        }

        currentDialogueIndex++;
        ShowNextDialogue();
    }
}