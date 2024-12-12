using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    public float delayTime = 6f;  // Delay time in seconds

    private bool sceneTriggered = false;  // To avoid multiple triggers

    void Start()
    {
        // Start automatic scene change after delay
        StartCoroutine(ChangeSceneAfterDelay());
    }

    void Update()
    {
        // Check for spacebar press to change the scene
        if (!sceneTriggered && Input.GetKeyDown(KeyCode.Space))
        {
            sceneTriggered = true;  // Prevent multiple scene triggers
            ChangeScene();
        }
    }

    private IEnumerator ChangeSceneAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);

        // Ensure the scene hasn't already changed by spacebar
        if (!sceneTriggered)
        {
            sceneTriggered = true;
            ChangeScene();
        }
    }

    private void ChangeScene()
    {
        SceneManager.LoadScene("informer evid");  // Change to the target scene
    }
}
