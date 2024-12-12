using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Act2 : MonoBehaviour
{
    public static Act2 Instance;

    // Character Images
    public Image badsarpanch, secretary, minister;

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
        nextButton.onClick.AddListener(CheckEndOrNextDialogue);

        UpdateNavigationButtons();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) CheckEndOrNextDialogue();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) ShowPreviousDialogue();
    }

    void UpdateNavigationButtons()
    {
        prevButton.interactable = currentDialogueIndex > 0 && canGoBack && !choicePanel.activeSelf;
        nextButton.interactable = !choicePanel.activeSelf;
    }

    public void CheckEndOrNextDialogue()
    {
        if (currentDialogueIndex >= dialogues.Length)
        {
            LoadNextScene();  // Load the next scene
        }
        else
        {
            ShowNextDialogue();  // Continue dialogue
        }
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
        ResetCharacterVisibility();  // Clear old character visibility

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

        switch (choiceIndex)
        {
            case 0:  
                DisplayDialogue(currentDialogue.choice1Dialogue);
                ShowCharacter(currentDialogue.choice1Character);
                UpdateSliders(-6, 6, 1);
                break;

            case 1:  
                DisplayDialogue(currentDialogue.choice2Dialogue);
                ShowCharacter(currentDialogue.choice2Character);
                UpdateSliders(0, -5, 0);
                break;

            case 2:  
                DisplayDialogue(currentDialogue.choice3Dialogue);
                ShowCharacter(currentDialogue.choice3Character);
                UpdateSliders(6, -6, -2);
                break;
        }

        currentDialogueIndex++;
        UpdateNavigationButtons();
        ShowNextDialogue();
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
        badsarpanch.enabled = false;
        secretary.enabled = false;
        minister.enabled = false;
    }

    private void ShowCharacter(string character)
    {
        ResetCharacterVisibility();

        switch (character.ToLower())  
        {
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

    private void LoadNextScene()
    {
        SceneManager.LoadScene("Real Act2");
    }
}
