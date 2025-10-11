using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CountdownTimer : MonoBehaviour
{
    [Header("UI Elements")]
    public Button startButton;           // Assign your start button
    public TextMeshProUGUI timerText;    // Assign your TextMeshProUGUI for the timer display

    [Header("Timer Settings")]
    public int startMinutes = 1;         // Minutes to start from
    public int startSeconds = 0;         // Seconds to start from

    private float remainingTime;         // Time left in seconds
    private bool isCountingDown = false;

    void Start()
    {
        if (startButton != null)
            startButton.onClick.AddListener(StartCountdown);

        // Initialize timer display
        remainingTime = startMinutes * 60 + startSeconds;
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (isCountingDown)
        {
            if (remainingTime > 0)
            {
                remainingTime -= Time.deltaTime;
                if (remainingTime < 0) remainingTime = 0; // Prevent negative time
                UpdateTimerDisplay();
            }
            else
            {
                isCountingDown = false;
            }
        }
    }

    void StartCountdown()
    {
        remainingTime = startMinutes * 60 + startSeconds;
        isCountingDown = true;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(remainingTime / 60f);
        int seconds = Mathf.FloorToInt(remainingTime % 60f);
        if (timerText != null)
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public void StopCountdown()
    {
        isCountingDown = false;
    }
}
