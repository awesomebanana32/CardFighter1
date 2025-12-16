using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    [Header("Database")]
    public ObjectDatabaseSO database;

    [Header("Gameplay")]
    public int objectIDToSpawn = 0;       // ID of the building/unit to spawn
    public Camera mainCamera;             // Drag your main camera here

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            SpawnAtMouse();
        }
    }

    void SpawnAtMouse()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned!");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast against everything; works with Terrain
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            // Only spawn if we hit a terrain
            if (hit.collider.GetComponent<Terrain>() == null)
            {
                Debug.Log("You clicked something that is not terrain.");
                return;
            }

            Vector3 spawnPos = hit.point;

            GameObject prefab = database.GetPrefabByID(objectIDToSpawn);
            int goldCost = database.GetGoldCostByID(objectIDToSpawn);

            if (prefab == null)
            {
                Debug.LogWarning("Prefab not found for object ID: " + objectIDToSpawn);
                return;
            }

            if (GoldManager.Instance.SpendGold(goldCost))
            {
                Instantiate(prefab, spawnPos, Quaternion.identity);
                Debug.Log($"Spawned {prefab.name} at {spawnPos}. Remaining gold: {GoldManager.Instance.CurrentGold}");
            }
            else
            {
                Debug.Log("Not enough gold!");
            }
        }
        else
        {
            Debug.Log("Raycast did not hit anything!");
        }
    }
}
