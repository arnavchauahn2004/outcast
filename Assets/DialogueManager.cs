using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;              // Dialogue text to display
    public GameObject dialogueBox;            // Dialogue box panel
    public GameObject choicePanel;            // Panel to display choices
    public TMP_Text[] choiceButtons;          // Buttons for choices
    public GameObject goodSarpanch;           // Good Sarpanch GameObject
    public GameObject badSarpanch;            // Bad Sarpanch GameObject
    public GameObject minister;               // Minister GameObject
    public GameObject secretary;              // Secretary GameObject
    public Slider moraleMeter;                // Slider for Morale Meter
    public Slider corruptionMeter;            // Slider for Corruption Meter
    public Slider partyStandingMeter;         // Slider for Party Standing

    private int currentDialogueIndex = 0;     // Current dialogue index
    private string[] dialogues;               // Array of dialogues
    private string[][] choices;               // Array of choices (text for display)
    private int[][] choiceImpacts;            // Array of impacts for each choice
    private int currentChoiceSet = 0;         // Current set of choices
    private GameObject currentCharacter;      // Active character GameObject

    void Start()
    {
        dialogueBox.SetActive(false);         // Start with dialogue box hidden
        choicePanel.SetActive(false);         // Start with choice panel hidden

        // Example dialogues
        dialogues = new string[]
        {
            "Good sarpanch (Satyaraj Yadav): minister Sahab - minister Sahab,We are from a small village Surajmukhi in the east. The road condition there is terrible and nobody can safely commute using the main road. Children can’t even go to school due to this. Monsoon is near if nothing is done the entire road will be full of water and cause so much problems to our community. We may be a poor village but if you help us our community will be truthful and loyal to you.",
            "Secretary: The Minister would like some time to think about it.",
            "Minister: I have a decision to make, but I need some time.",
            "Bad Sarpanch: Minister Sahab, let’s work together for mutual benefit."
        };

        // Choices for specific dialogue points (text visible to player)
        choices = new string[][]
        {
            new string[] { "I understand your pain it’s a big issue I will surely do something about it.", " I will need to review the current budget and infrastructure plans to see how we can incorporate this request.", "we have many previous pending commitments I will se what can be done." },
            new string[] { "Order an immediate investigation", "Choose according to your will", "Ignore" },
            new string[] { "Directly Decline", "Leave it on the table and go" }
        };

        // Impacts for each choice on meters [morale, corruption, partyStanding]
        choiceImpacts = new int[][]
        {
            new int[] { 10, 0, 5 },   // Good
            new int[] { 5, 0, 3 },    // Diplomatic
            new int[] { -5, 10, 2 }   // Bad
        };

        // Initially set the good Sarpanch as the active character
        ShowCharacter(goodSarpanch);
        ShowDialogue();
    }

    void Update()
    {
        if (dialogueBox.activeSelf && Input.GetMouseButtonDown(0) && !choicePanel.activeSelf)
        {
            ShowDialogue();
        }
    }

    public void ShowDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            string dialogue = dialogues[currentDialogueIndex];

            // Parse the speaker and dialogue
            if (dialogue.StartsWith("Good Sarpanch:"))
            {
                ShowCharacter(goodSarpanch);
            }
            else if (dialogue.StartsWith("Secretary:"))
            {
                ShowCharacter(secretary);
                choicePanel.SetActive(false);  // Hide choice panel for Secretary's dialogue
            }
            else if (dialogue.StartsWith("Bad Sarpanch:"))
            {
                ShowCharacter(badSarpanch);
                choicePanel.SetActive(false);  // Hide choice panel for Bad Sarpanch's dialogue
            }
            else if (dialogue.StartsWith("Minister:"))
            {
                ShowCharacter(minister);
                ShowChoices();  // Show choices panel for Minister's dialogue
            }

            dialogueBox.SetActive(true);
            dialogueText.text = dialogue.Split(':')[1].Trim();

            currentDialogueIndex++; // Move to next dialogue
        }
        else
        {
            dialogueBox.SetActive(false);  // End dialogue
        }
    }

    void ShowChoices()
    {
        choicePanel.SetActive(true);  // Show choice panel
        string[] currentChoices = choices[currentChoiceSet];

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < currentChoices.Length)
            {
                choiceButtons[i].transform.parent.gameObject.SetActive(true);  // Show choice button
                choiceButtons[i].text = currentChoices[i];  // Display text on the button
            }
            else
            {
                choiceButtons[i].transform.parent.gameObject.SetActive(false);  // Hide unused buttons
            }
        }

        currentChoiceSet++; // Move to next set of choices
    }

    public void OnChoiceSelected(int index)
    {
        int[] impact = choiceImpacts[index]; // Get meter impacts for selected choice

        UpdateMeters(impact[0], impact[1], impact[2]);

        // After a choice is selected, update the dialogue to show "Minister (you): [choice]"
        string playerChoice = choices[currentChoiceSet - 1][index];
        dialogueText.text = "Minister (you): " + playerChoice; // Display the choice as Minister's dialogue

        choicePanel.SetActive(false);  // Hide choice panel after selection

        // Optionally, you can move on to the next dialogue after showing the choice
        Invoke("ShowDialogue", 2f); // Delay to simulate the transition
    }

    void UpdateMeters(int morale, int corruption, int partyStanding)
    {
        moraleMeter.value += morale;
        corruptionMeter.value += corruption;
        partyStandingMeter.value += partyStanding;
    }

    void ShowCharacter(GameObject character)
    {
        // Hide the current active character
        if (currentCharacter != null)
        {
            currentCharacter.SetActive(false);
        }

        // Show the new character
        currentCharacter = character;
        currentCharacter.SetActive(true);
    }
}