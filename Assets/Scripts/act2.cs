using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Act2Manager : MonoBehaviour
{
    // UI and meters
    public Image minister, secretary, police, parents, informer;
    public Slider corruptionMeter, moraleMeter, politicalStandingMeter;
    public TextMeshProUGUI dialogueText;
    public GameObject choicePanel;
    public Button[] choiceButtons;

    public Button prevButton; // Button to go to the previous dialogue
    public Button nextButton; // Button to go to the next dialogue manually

    private int currentDialogueIndex = 0;
    private int playerChoice = -1; // -1 means no choice yet

    // Character-specific images and audio
    [System.Serializable]
    public struct CharacterAssets
    {
        public Sprite[] expressions; // Array of expression images
        public AudioClip[] voiceClips; // Array of voice audio files
    }

    public CharacterAssets ministerAssets, secretaryAssets, policeAssets, parentsAssets, informerAssets;

    [System.Serializable]
    public struct Dialogue
    {
        public string character; // Name of the character
        public string text;      // Dialogue text
        public bool hasChoices;  // Whether the dialogue has choices
        public string[] choices; // Choices for the dialogue
        public int[] nextDialogueIndexes; // Dialogue indexes for each choice
        public int expressionIndex; // Index of the character's expression
        public int voiceIndex;      // Index of the character's voice clip
    }

    public Dialogue[] dialogues;

    private AudioSource audioSource;

    void Start()
    {
        // Initialize components
        choicePanel.SetActive(false);
        audioSource = GetComponent<AudioSource>();

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
        ResetCharacterVisibility();

        // Handle choices
        if (currentDialogue.hasChoices)
        {
            dialogueText.text = ""; // No text shown before the choice
            ShowChoices(currentDialogue.choices, currentDialogue.nextDialogueIndexes);
            UpdateNavigationButtons(); // Update buttons after choices are displayed
            return;
        }

        // Set dialogue text
        dialogueText.text = currentDialogue.text;

        // Display character and play audio
        DisplayCharacter(currentDialogue.character, currentDialogue.expressionIndex, currentDialogue.voiceIndex);

        currentDialogueIndex++;
        UpdateNavigationButtons(); // Update buttons after dialogue change
    }

    public void ShowPreviousDialogue()
    {
        if (currentDialogueIndex <= 0) return;

        currentDialogueIndex--;

        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        // Reset character visibility
        ResetCharacterVisibility();

        // Set dialogue text
        dialogueText.text = currentDialogue.text;

        // Display character and play audio
        DisplayCharacter(currentDialogue.character, currentDialogue.expressionIndex, currentDialogue.voiceIndex);

        UpdateNavigationButtons(); // Update buttons after dialogue change
    }

    private void ShowChoices(string[] choices, int[] nextDialogueIndexes)
    {
        choicePanel.SetActive(true);

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => HandleChoice(index, nextDialogueIndexes[index]));
        }

        // Deactivate unused buttons
        for (int i = choices.Length; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void HandleChoice(int choiceIndex, int nextDialogueIndex)
    {
        playerChoice = choiceIndex; // Capture the player's choice
        choicePanel.SetActive(false);

        // Display the selected choice as dialogue text
        dialogueText.text = dialogues[currentDialogueIndex].choices[choiceIndex];

        // Adjust sliders based on choice (optional consequences logic)
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

        // Navigate to the dialogue index specified for the chosen option
        currentDialogueIndex = nextDialogueIndex;
        UpdateNavigationButtons(); // Update buttons after choice
        ShowNextDialogue();
    }

    private void ResetCharacterVisibility()
    {
        minister.enabled = false;
        secretary.enabled = false;
        police.enabled = false;
        parents.enabled = false;
        informer.enabled = false;
    }

    private void DisplayCharacter(string character, int expressionIndex, int voiceIndex)
    {
        CharacterAssets assets = GetCharacterAssets(character);
        Image characterImage = GetCharacterImage(character);

        if (assets.expressions.Length > expressionIndex)
        {
            characterImage.sprite = assets.expressions[expressionIndex];
        }

        characterImage.enabled = true;

        if (assets.voiceClips.Length > voiceIndex && audioSource != null)
        {
            audioSource.clip = assets.voiceClips[voiceIndex];
            audioSource.Play();
        }
    }

    private CharacterAssets GetCharacterAssets(string character)
    {
        switch (character)
        {
            case "minister": return ministerAssets;
            case "secretary": return secretaryAssets;
            case "police": return policeAssets;
            case "parents": return parentsAssets;
            case "informer": return informerAssets;
            default: return new CharacterAssets();
        }
    }

    private Image GetCharacterImage(string character)
    {
        switch (character)
        {
            case "minister": return minister;
            case "secretary": return secretary;
            case "police": return police;
            case "parents": return parents;
            case "informer": return informer;
            default: return null;
        }
    }
}