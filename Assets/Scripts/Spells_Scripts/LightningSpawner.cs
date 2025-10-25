using UnityEngine;

public class LightningSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    public GameObject lightningPrefab;
    public Camera mainCamera;

    private bool lightningModeActive = false;

    // Call this from your UI Button OnClick
    public void EnableLightningMode()
    {
        lightningModeActive = true;
    }

    void Update()
    {
        if (!lightningModeActive) return;

        // Right-click detection
        if (Input.GetMouseButtonDown(1)) // 1 = right mouse button
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SpawnLightning(hit.point);
            }
        }
    }

    void SpawnLightning(Vector3 position)
    {
        if (lightningPrefab != null)
        {
            Instantiate(lightningPrefab, position, Quaternion.identity);
        }

        // Disable lightning mode after one spawn
        lightningModeActive = false;
    }
}
