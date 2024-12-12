using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneManager : MonoBehaviour
{
    public float delayBeforeNextScene = 4f; // Time in seconds before automatic transition
    public string nextSceneName = "Act2"; // Name of the next scene
    private bool hasTransitioned = false; // Ensure we don't transition multiple times

    void Start()
    {
        // Start the countdown for the automatic transition
        StartCoroutine(WaitAndTransition());
    }

    void Update()
    {
        // Allow skipping to the next scene by pressing Space
        if (Input.GetKeyDown(KeyCode.Space) && !hasTransitioned)
        {
            hasTransitioned = true; // Prevent multiple transitions
            TransitionToNextScene();
        }
    }

    private IEnumerator WaitAndTransition()
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delayBeforeNextScene);

        if (!hasTransitioned) // Only transition if Space hasn't been pressed
        {
            hasTransitioned = true;
            TransitionToNextScene();
        }
    }

    private void TransitionToNextScene()
    {
        // Load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
