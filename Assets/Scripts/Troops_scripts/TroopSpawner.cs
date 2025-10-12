using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TroopSpawner : MonoBehaviour
{
    [Header("Troop Settings")]
    [SerializeField] private GameObject troopPrefab; // The troop prefab to spawn
    [SerializeField] private Transform spawnPoint;   // Where troops will spawn

    [Header("Spawn Button")]
    [SerializeField] private string buttonName = "SpawnButton"; // Name of the button in the scene

    [Header("Spawn Timing")]
    [SerializeField] private float minSpawnInterval = 1f; // Minimum time between spawns
    [SerializeField] private float maxSpawnInterval = 3f; // Maximum time between spawns

    private Button spawnButton; // Reference to the button
    private bool isSpawning = false; // Tracks if spawning is active

    void Start()
    {
        // Find the button in the scene by name
        GameObject buttonObject = GameObject.Find(buttonName);
        if (buttonObject != null)
        {
            spawnButton = buttonObject.GetComponent<Button>();
            if (spawnButton != null)
            {
                spawnButton.onClick.AddListener(StartSpawning);
            }
            else
            {
                Debug.LogError($"No Button component found on GameObject '{buttonName}'!");
            }
        }
        else
        {
            Debug.LogError($"No GameObject named '{buttonName}' found in the scene!");
        }
    }

    void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            StartCoroutine(SpawnTroops());
        }
    }

    IEnumerator SpawnTroops()
    {
        while (isSpawning)
        {
            // Check if troop prefab and spawn point are assigned
            if (troopPrefab != null && spawnPoint != null)
            {
                // Spawn troop at spawn point with prefab's rotation
                Instantiate(troopPrefab, spawnPoint.position, troopPrefab.transform.rotation);

                // Wait for a random time between min and max before spawning the next troop
                float randomWait = Random.Range(minSpawnInterval, maxSpawnInterval);
                yield return new WaitForSeconds(randomWait);
            }
            else
            {
                Debug.LogWarning("Troop Prefab or Spawn Point not assigned!");
                isSpawning = false;
                yield break;
            }
        }
    }

    // Optional: Method to stop spawning manually
    public void StopSpawning()
    {
        isSpawning = false;
    }
}
