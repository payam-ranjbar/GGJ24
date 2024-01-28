using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TIMERBOMB : MonoBehaviour
{
    public Slider slider; // Reference to the slider in the Unity Editor
    public TMP_Text countdownText; // Reference to the TextMeshPro component
    public float countdownDuration = 10f; // Duration of the countdown in seconds

    private float currentTime;

    void Start()
    {
        currentTime = countdownDuration;
    }

    void Update()
    {
        // Update the countdown
        currentTime -= Time.deltaTime;

        // Ensure the countdown doesn't go below 0
        currentTime = Mathf.Max(0, currentTime);

        // Update the slider value based on the countdown progress
        slider.value = currentTime / countdownDuration;

        // Update the text display
        UpdateCountdownText();
    }

    void UpdateCountdownText()
    {
        // Format the time as an integer for display
        int timeRemaining = Mathf.CeilToInt(currentTime);
        countdownText.text = timeRemaining.ToString();
    }
}
