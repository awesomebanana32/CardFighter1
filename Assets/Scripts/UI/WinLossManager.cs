using UnityEngine;

public class CapitalCityManager : MonoBehaviour
{
    [Header("UI Screens")]
    public GameObject winScreen;   // Assign Win UI panel
    public GameObject loseScreen;  // Assign Lose UI panel

    [Header("Capital Cities")]
    public GameObject greenCapital; // Your team's capital
    public GameObject redCapital;   // Enemy's capital

    private bool gameEnded = false;

    void Start()
    {
        // Hide UI screens at start
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
    }

    // Call this method when any capital is destroyed
    public void CapitalDestroyed(GameObject destroyedCapital)
    {
        if (gameEnded) return;

        gameEnded = true;
        Time.timeScale = 0f; // Pause the game

        if (destroyedCapital == greenCapital)
        {
            // Player's capital destroyed  lose
            if (loseScreen != null) loseScreen.SetActive(true);
        }
        else if (destroyedCapital == redCapital)
        {
            // Enemy capital destroyed  win
            if (winScreen != null) winScreen.SetActive(true);
        }
    }
}
