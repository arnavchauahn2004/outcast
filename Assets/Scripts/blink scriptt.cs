using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MeterBlink : MonoBehaviour
{
    public Image meterFill;                  // The Image component of the meter's fill
    public Color highlightColor = Color.white;  // Blink color (slightly brighter)
    public float blinkDuration = 5f;        // Blink duration increased from 3 to 5 seconds
    public float blinkSpeed = 0.5f;         // Blink interval

    private Color originalColor;            // Store original color
    private bool isBlinking = false;

    void Start()
    {
        if (meterFill != null)
        {
            originalColor = meterFill.color;  // Save the original color
        }
    }

    public void StartBlink()
    {
        if (!isBlinking && meterFill != null)
        {
            StartCoroutine(BlinkMeter());
        }
    }

    IEnumerator BlinkMeter()
    {
        isBlinking = true;
        float elapsed = 0f;

        while (elapsed < blinkDuration)
        {
            // Blink ON (highlight color)
            meterFill.color = highlightColor;
            yield return new WaitForSeconds(blinkSpeed);

            // Blink OFF (original color)
            meterFill.color = originalColor;
            yield return new WaitForSeconds(blinkSpeed);

            elapsed += blinkSpeed * 2f;  // Total elapsed time after both intervals
        }

        meterFill.color = originalColor;  // Reset to original color after blinking
        isBlinking = false;
    }
}