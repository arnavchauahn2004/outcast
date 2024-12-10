using UnityEngine;
using UnityEngine.UI;

public class Act2Manager : MonoBehaviour
{
    public Slider corruptionMeter, moraleMeter, politicalStandingMeter;

    void Start()
    {
        // Load meter values from GameManager
        if (GameManager.Instance != null)
        {
            corruptionMeter.value = GameManager.Instance.corruptionMeterValue;
            moraleMeter.value = GameManager.Instance.moraleMeterValue;
            politicalStandingMeter.value = GameManager.Instance.politicalStandingMeterValue;
        }
        else
        {
            Debug.LogWarning("GameManager instance not found. Meters will use default values.");
        }
    }

    public void PerformAction(string action)
    {
        switch (action)
        {
            case "boostMorale":
                moraleMeter.value = Mathf.Clamp(moraleMeter.value + 10, 0, 100);
                break;

            case "increaseCorruption":
                corruptionMeter.value = Mathf.Clamp(corruptionMeter.value + 10, 0, 100);
                break;

            case "improvePoliticalStanding":
                politicalStandingMeter.value = Mathf.Clamp(politicalStandingMeter.value + 10, 0, 100);
                break;

            default:
                Debug.LogWarning("Unknown action: " + action);
                break;
        }

        // Save the updated meter values back to GameManager
        SaveMeterValues();
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
}
