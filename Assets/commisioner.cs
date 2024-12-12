using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CommissionerSceneManager : MonoBehaviour
{
    public static CommissionerSceneManager Instance;

    // Character Images
    public Image minister, secretary, commissioner;

    // UI Elements
    public Slider corruptionMeter, moraleMeter, politicalStandingMeter;
    public TextMeshProUGUI dialogueText;
    public GameObject choicePanel;
    public Button[] choiceButtons;
    public Button prevButton, nextButton;

    private int currentDialogueIndex = 0;
    private Coroutine typingCoroutine;
    private bool canGoBack = true;

    // Persistent meter values
    public float corruptionMeterValue = 0f;
    public float moraleMeterValue = 0f;
    public float politicalStandingMeterValue = 0f;

    [System.Serializable]
    public struct CharacterDialogue
    {
        public string character;            // Character name
        public string text;                 // Main dialogue text
        public bool hasChoices;             // Whether choices appear
        public string[] choices;            // Choice options

        public string choice1Dialogue;      // Response for Choice 1
        public string choice2Dialogue;      // Response for Choice 2
        public string choice3Dialogue;      // Response for Choice 3

        public string choice1Character;     // Character speaking for Choice 1
        public string choice2Character;     // Character speaking for Choice 2
        public string choice3Character;     // Character speaking for Choice 3

        public string nextSceneChoice1;     // Scene after Choice 1
        public string nextSceneChoice2;     // Scene after Choice 2
        public string nextSceneChoice3;     // Scene after Choice 3
    }

    public CharacterDialogue[] dialogues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        InitializeMeters();
        SetupButtons();
        ShowNextDialogue();
    }

    void InitializeMeters()
    {
        if (GameManager.Instance != null)
        {
            corruptionMeter.value = GameManager.Instance.corruptionMeterValue;
            moraleMeter.value = GameManager.Instance.moraleMeterValue;
            politicalStandingMeter.value = GameManager.Instance.politicalStandingMeterValue;
        }
        else
        {
            Debug.LogWarning("GameManager not found, initializing default meter values.");
        }
    }

    void SetupButtons()
    {
        choicePanel.SetActive(false);

        prevButton.onClick.AddListener(ShowPreviousDialogue);
        nextButton.onClick.AddListener(ShowNextDialogue);

        UpdateNavigationButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)) ShowNextDialogue();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowPreviousDialogue();
    }

    void UpdateNavigationButtons()
    {
        prevButton.interactable = currentDialogueIndex > 0 && canGoBack && !choicePanel.activeSelf;
        nextButton.interactable = !choicePanel.activeSelf;
    }

    public void ShowNextDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length) return;

        CharacterDialogue currentDialogue = dialogues[currentDialogueIndex];
        ResetCharacterVisibility();

        if (currentDialogue.hasChoices)
        {
            dialogueText.text = "";
            ShowChoices(currentDialogue.choices);
            UpdateNavigationButtons();
            return;
        }

        DisplayDialogue(currentDialogue.text);
        ShowCharacter(currentDialogue.character);

        currentDialogueIndex++;
        canGoBack = false;
        UpdateNavigationButtons();
    }

    public void ShowPreviousDialogue()
    {
        if (!canGoBack || currentDialogueIndex <= 0 || choicePanel.activeSelf) return;

        currentDialogueIndex--;
        CharacterDialogue currentDialogue = dialogues[currentDialogueIndex];

        ResetCharacterVisibility();
        DisplayDialogue(currentDialogue.text);
        ShowCharacter(currentDialogue.character);
        UpdateNavigationButtons();
    }

    private void DisplayDialogue(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void ShowChoices(string[] choices)
    {
        choicePanel.SetActive(true);
        ResetCharacterVisibility();

        // Ensure Minister is enabled when choices are displayed
        minister.enabled = true;

        prevButton.interactable = false;
        nextButton.interactable = false;

        for (int i = 0; i < choices.Length; i++)
        {
            int index = i;
            choiceButtons[i].gameObject.SetActive(true);
            choiceButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = choices[i];
            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(() => HandleChoice(index));
        }

        for (int i = choices.Length; i < choiceButtons.Length; i++)
        {
            choiceButtons[i].gameObject.SetActive(false);
        }
    }

    private void HandleChoice(int choiceIndex)
    {
        CharacterDialogue currentDialogue = dialogues[currentDialogueIndex];
        choicePanel.SetActive(false);
        canGoBack = false;
        nextButton.interactable = true;

        string nextScene = null;

        switch (choiceIndex)
        {
            case 0: // First Choice
                DisplayDialogue(currentDialogue.choice1Dialogue);
                ShowCharacter(currentDialogue.choice1Character);
                UpdateSliders(-6, 6, 1);
                nextScene = currentDialogue.nextSceneChoice1;
                break;

            case 1: // Second Choice
                DisplayDialogue(currentDialogue.choice2Dialogue);
                ShowCharacter(currentDialogue.choice2Character);
                UpdateSliders(0, -5, 0);
                nextScene = currentDialogue.nextSceneChoice2;
                break;

            case 2: // Third Choice
                DisplayDialogue(currentDialogue.choice3Dialogue);
                ShowCharacter(currentDialogue.choice3Character);
                UpdateSliders(6, -6, -2);
                nextScene = currentDialogue.nextSceneChoice3;
                break;
        }

        currentDialogueIndex++;
        UpdateNavigationButtons();

        // Handle scene change if applicable
        if (!string.IsNullOrEmpty(nextScene))
        {
            StartCoroutine(HandleSceneTransition(nextScene));
        }
        else
        {
            ShowNextDialogue();
        }
    }

    private IEnumerator HandleSceneTransition(string sceneName)
    {
        yield return new WaitForSeconds(2f); // Wait for dialogue completion
        SceneManager.LoadScene(sceneName);
    }

    private void UpdateSliders(int corruptionChange, int moraleChange, int politicalChange)
{
    int maxChange = Mathf.Max(corruptionChange, moraleChange, politicalChange);

    if (maxChange > 0)
    {
        if (maxChange == corruptionChange)
        {
            corruptionMeter.value = Mathf.Clamp(corruptionMeter.value + corruptionChange, 0, 100);
            corruptionMeter.GetComponent<MeterBlink>().StartBlink();
        }
        else if (maxChange == moraleChange)
        {
            moraleMeter.value = Mathf.Clamp(moraleMeter.value + moraleChange, 0, 100);
            moraleMeter.GetComponent<MeterBlink>().StartBlink();
        }
        else if (maxChange == politicalChange)
        {
            politicalStandingMeter.value = Mathf.Clamp(politicalStandingMeter.value + politicalChange, 0, 100);
            politicalStandingMeter.GetComponent<MeterBlink>().StartBlink();
        }
    }

    SaveMeterValues();  // Save updated values
}


    private void SaveMeterValues()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.corruptionMeterValue = corruptionMeter.value;
            GameManager.Instance.moraleMeterValue = moraleMeter.value;
            GameManager.Instance.politicalStandingMeterValue = politicalStandingMeter.value;
        }
    }

    private void ResetCharacterVisibility()
    {
        minister.enabled = false;
        secretary.enabled = false;
        commissioner.enabled = false;
    }

    private void ShowCharacter(string character)
    {
        switch (character.ToLower())
        {
            case "minister": minister.enabled = true; break;
            case "secretary": secretary.enabled = true; break;
            case "commissioner": commissioner.enabled = true; break;
            default: Debug.LogWarning($"Character not recognized: '{character}'"); break;
        }
    }
}
