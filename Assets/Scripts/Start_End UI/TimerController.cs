using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountUpTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;           // Assign your start button
    public TextMeshProUGUI timerText;    // Assign your TextMeshProUGUI for the timer display

    [Header("Timer Settings")]
    public int targetMinutes = 1;        // Optional: Stop at this minute mark
    public int targetSeconds = 0;        // Optional: Stop at this second mark
    public bool stopAtTargetTime = false; // If true, timer stops when reaching target time

    private float elapsedTime = 0f;      // Time passed in seconds
    private bool isCounting = false;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartCount);

        // Initialize timer display
        elapsedTime = 0f;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isCounting)
        {
            elapsedTime += Time.deltaTime;

            if (stopAtTargetTime)
            {
                float targetTime = targetMinutes * 60 + targetSeconds;
                if (elapsedTime >= targetTime)
                {
                    elapsedTime = targetTime;
                    isCounting = false;
                }
            }

            UpdateTimerDisplay();
        }
    }

    void StartCount()
    {
        elapsedTime = 0f;
        isCounting = true;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopCount()
    {
        isCounting = false;
    }
}
