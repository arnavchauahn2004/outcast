using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton instance

    public Image goodsarpanch, badsarpanch, secretary, minister;
    public Slider corruptionMeter, moraleMeter, politicalStandingMeter;
    public TextMeshProUGUI dialogueText;
    public GameObject choicePanel, subChoicePanel;
    public Button[] choiceButtons;

    public Button goodSarpanchButton, badSarpanchButton;
    public Button prevButton, nextButton;

    private int currentDialogueIndex = 0;
    private Coroutine typingCoroutine; // Store reference for typing coroutine

    // Persistent meter values
    public float corruptionMeterValue = 0f;
    public float moraleMeterValue = 0f;
    public float politicalStandingMeterValue = 0f;

    private bool canGoBack = true; // Track whether backward navigation is allowed

    [System.Serializable]
    public struct Dialogue
    {
        public string character;
        public string text;
        public bool hasChoices;
        public string[] choices;

        public string goodChoiceDialogue;  // Response for Choice 0
        public string moderateChoiceDialogue;  // Response for Choice 1
        public string corruptChoiceDialogue;  // Response for Choice 2

        public string goodChoiceCharacter;  // Speaker for Choice 0
        public string moderateChoiceCharacter;  // Speaker for Choice 1
        public string corruptChoiceCharacter;  // Speaker for Choice 2

        public bool hasSubChoice;  // Check for nested sub-choice
    }

    public Dialogue[] dialogues;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);  // Persist across scenes
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
        if (corruptionMeter != null) corruptionMeter.value = corruptionMeterValue;
        if (moraleMeter != null) moraleMeter.value = moraleMeterValue;
        if (politicalStandingMeter != null) politicalStandingMeter.value = politicalStandingMeterValue;
    }

    void SetupButtons()
    {
        choicePanel.SetActive(false);
        subChoicePanel.SetActive(false);

        prevButton.onClick.AddListener(ShowPreviousDialogue);
        nextButton.onClick.AddListener(ShowNextDialogue);

        goodSarpanchButton.onClick.AddListener(() => StartCoroutine(HandleGoodSarpanchChoice()));
        badSarpanchButton.onClick.AddListener(() => StartCoroutine(HandleBadSarpanchChoice()));

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

        Dialogue currentDialogue = dialogues[currentDialogueIndex];
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
        canGoBack = false;  // Disable backward navigation after moving to next dialogue
        UpdateNavigationButtons();
    }

    public void ShowPreviousDialogue()
    {
        if (!canGoBack || currentDialogueIndex <= 0 || choicePanel.activeSelf) return;

        currentDialogueIndex--;
        Dialogue currentDialogue = dialogues[currentDialogueIndex];

        ResetCharacterVisibility();
        DisplayDialogue(currentDialogue.text);
        ShowCharacter(currentDialogue.character);
        UpdateNavigationButtons();
    }

    private void DisplayDialogue(string text)
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);  // Stop previous typing effect if running
        }
        typingCoroutine = StartCoroutine(TypeText(text));
    }

    IEnumerator TypeText(string text)
    {
        dialogueText.text = "";  // Clear the previous text
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;  // Add each character one by one
            yield return new WaitForSeconds(0.05f);  // Typing speed
        }
    }

    private void ShowChoices(string[] choices)
    {
        choicePanel.SetActive(true);
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
        Dialogue currentDialogue = dialogues[currentDialogueIndex];
        choicePanel.SetActive(false);
        canGoBack = false;  // Disable backward navigation after choice is made
        nextButton.interactable = true;

        switch (choiceIndex)
        {
            case 0:  // Good Choice
                DisplayDialogue(currentDialogue.goodChoiceDialogue);
                ShowCharacter(currentDialogue.goodChoiceCharacter);
                UpdateSliders(-6, 6, 1);
                break;
            case 1:  // Moderate Choice
                DisplayDialogue(currentDialogue.moderateChoiceDialogue);
                ShowCharacter(currentDialogue.moderateChoiceCharacter);
                if (currentDialogue.hasSubChoice)
                {
                    subChoicePanel.SetActive(true);
                    return;
                }
                UpdateSliders(0, -5, 0);
                break;
            case 2:  // Corrupt Choice
                DisplayDialogue(currentDialogue.corruptChoiceDialogue);
                ShowCharacter(currentDialogue.corruptChoiceCharacter);
                UpdateSliders(6, -6, -2);
                break;
        }

        currentDialogueIndex++;
        UpdateNavigationButtons();
        ShowNextDialogue();
    }

    private IEnumerator HandleGoodSarpanchChoice()
    {
        DisplayDialogue("You selected the Good Sarpanch.");
        moraleMeter.value = Mathf.Clamp(moraleMeter.value + 5, 0, 100);
        subChoicePanel.SetActive(false);
        yield return new WaitForSeconds(2f);  // Wait for dialogue completion
        SaveMeterValues();
        SceneManager.LoadScene("Act2");
    }

    private IEnumerator HandleBadSarpanchChoice()
    {
        DisplayDialogue("You chose the Bad Sarpanch.");
        corruptionMeter.value = Mathf.Clamp(corruptionMeter.value + 5, 0, 100);
        politicalStandingMeter.value = Mathf.Clamp(politicalStandingMeter.value - 4, 0, 100);
        subChoicePanel.SetActive(false);
        yield return new WaitForSeconds(1.5f);  // Wait for dialogue completion
    }

    private void SaveMeterValues()
    {
        corruptionMeterValue = corruptionMeter.value;
        moraleMeterValue = moraleMeter.value;
        politicalStandingMeterValue = politicalStandingMeter.value;
    }

    private void UpdateSliders(int corruptionChange, int moraleChange, int politicalChange)
    {
        corruptionMeter.value = Mathf.Clamp(corruptionMeter.value + corruptionChange, 0, 100);
        moraleMeter.value = Mathf.Clamp(moraleMeter.value + moraleChange, 0, 100);
        politicalStandingMeter.value = Mathf.Clamp(politicalStandingMeter.value + politicalChange, 0, 100);
    }

    private void ResetCharacterVisibility()
    {
        goodsarpanch.enabled = false;
        badsarpanch.enabled = false;
        secretary.enabled = false;
        minister.enabled = false;
    }

    private void ShowCharacter(string character)
    {
        switch (character)
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
            default:
                Debug.LogWarning($"Character not recognized: '{character}'");
                break;
        }
    }
}
