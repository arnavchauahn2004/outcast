using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;   // Reference to the dialogue text
    public float typingSpeed = 0.05f;      // Speed of text appearing

    private Coroutine typingCoroutine;

    // Method to Start Typing Dialogue
    public void DisplayDialogue(string dialogue)
    {
        // Stop previous typing if ongoing
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // Start new typing effect
        typingCoroutine = StartCoroutine(TypeDialogue(dialogue));
    }

    // Typing Effect Coroutine
    IEnumerator TypeDialogue(string dialogue)
    {
        dialogueText.text = "";  // Clear previous text

        foreach (char letter in dialogue.ToCharArray())
        {
            dialogueText.text += letter;  // Add letter one by one
            yield return new WaitForSeconds(typingSpeed);  // Wait for typing speed
        }
    }
}