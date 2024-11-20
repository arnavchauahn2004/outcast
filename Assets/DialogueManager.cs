using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TMP_Text dialogueText;   // Assign in Inspector
    public GameObject dialogueBox; // Assign the DialogueBox panel

    private string[] dialogues;    // Array to hold dialogues
    private int currentDialogueIndex = 0; // Track current dialogue

    void Start()
    {
        dialogueBox.SetActive(false); // Start with the dialogue box hidden

        // Example dialogues
        dialogues = new string[]
        {
            "Welcome to the game!",
            "You are about to embark on a journey.",
            "Make wise decisions and good luck!"
        };

        ShowNextDialogue(); // Start the first dialogue
    }

    void Update()
    {
        // Check for user input (e.g., a tap or button press)
        if (dialogueBox.activeSelf && Input.GetMouseButtonDown(0)) // Left mouse button or tap
        {
            ShowNextDialogue();
        }
    }

    public void ShowNextDialogue()
    {
        if (currentDialogueIndex < dialogues.Length)
        {
            dialogueBox.SetActive(true);                  // Show the dialogue box
            dialogueText.text = dialogues[currentDialogueIndex]; // Set current dialogue text
            currentDialogueIndex++;                      // Move to the next dialogue
        }
        else
        {
            HideDialogue(); // Hide dialogue box if all dialogues are shown
        }
    }

    void HideDialogue()
    {
        dialogueBox.SetActive(false);
    }
}