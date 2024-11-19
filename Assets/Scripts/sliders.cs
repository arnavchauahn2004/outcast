using UnityEngine;
using UnityEngine.UI;

public class MeterController : MonoBehaviour
{
    public Slider corruptionMeter;  // Assign in the Inspector
    public Slider moralMeter;       // Assign in the Inspector
    public Slider PoliticalStanding;

    private int maxValue = 100;     // Max value for both meters

    void Start()
    {
        // Initialize sliders
        corruptionMeter.maxValue = maxValue;
        moralMeter.maxValue = maxValue;
        PoliticalStanding.maxValue = maxValue;
        corruptionMeter.value = 0;  // Start with no corruption
        moralMeter.value = 0; // Start with no morality
        PoliticalStanding.value = 0;
    }

    public void UpdateMeters(bool ethicalDecision)
    {
        if (ethicalDecision)
        {
            moralMeter.value = Mathf.Min(moralMeter.value + 10, maxValue);
            corruptionMeter.value = Mathf.Max(corruptionMeter.value - 5, 0);
            PoliticalStanding.value = Mathf.Min(PoliticalStanding.value + 5, maxValue);
        }
        else
        {
            moralMeter.value = Mathf.Max(moralMeter.value - 6, 0);
            corruptionMeter.value = Mathf.Min(corruptionMeter.value + 10, maxValue);
            PoliticalStanding.value = Mathf.Max(PoliticalStanding.value - 5, 0);
        }
    }
}