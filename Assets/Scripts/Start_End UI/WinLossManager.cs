using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class WinLossManager : MonoBehaviour
{
    public GameObject winScreen; // Assign Win UI panel in Inspector
    public GameObject loseScreen; // Assign Lose UI panel in Inspector
    private bool gameEnded = false;

    void Start()
    {
        // Ensure UI screens are initially hidden
        if (winScreen != null) winScreen.SetActive(false);
        if (loseScreen != null) loseScreen.SetActive(false);
    }

    // Public method to be called by the button's OnClick event
    public void StartTeamStatusCheck()
    {
        StartCoroutine(StartCheckWithDelay());
    }

    IEnumerator StartCheckWithDelay()
    {
        // Wait for 1 second before starting the team status check
        yield return new WaitForSeconds(1f);
        StartCoroutine(CheckTeamStatus());
    }

    IEnumerator CheckTeamStatus()
    {
        while (!gameEnded)
        {
            // Find all GameObjects with specified tags
            GameObject[] teamGreen = GameObject.FindGameObjectsWithTag("TeamGreen");
            GameObject[] teamRed = GameObject.FindGameObjectsWithTag("TeamRed");

            // Check if either team has zero members
            if (teamGreen.Length == 0)
            {
                gameEnded = true;
                Time.timeScale = 0f; // Pause the game
                if (loseScreen != null) loseScreen.SetActive(true);
            }
            else if (teamRed.Length == 0)
            {
                gameEnded = true;
                Time.timeScale = 0f; // Pause the game
                if (winScreen != null) winScreen.SetActive(true);
            }

            // Wait for 1 second before next check
            yield return new WaitForSeconds(1f);
        }
    }
}