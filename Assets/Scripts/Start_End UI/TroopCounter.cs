using UnityEngine;
using System.Collections;

public class TroopCounter : MonoBehaviour
{
    [SerializeField] private float updateInterval = 1f; // Update every 1 second

    private void Start()
    {
        StartCoroutine(UpdateTroopCounts());
    }

    private IEnumerator UpdateTroopCounts()
    {
        while (true)
        {
            int greenCount = GameObject.FindGameObjectsWithTag("TeamGreen").Length;
            int redCount = GameObject.FindGameObjectsWithTag("TeamRed").Length;

            Debug.Log("Team Green: " + greenCount + " | Team Red: " + redCount);

            yield return new WaitForSeconds(updateInterval);
        }
    }
}
