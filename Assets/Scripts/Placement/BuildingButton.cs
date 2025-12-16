using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    [Header("Database")]
    public ObjectDatabaseSO database;

    [Header("Gameplay")]
    public int objectIDToSpawn = 0;
    public Camera mainCamera;

    [Header("Preview")]
    public GameObject previewPrefab;

    private GameObject previewObject;
    private bool isPlacing = false;
    private float previewHeightOffset;

    void Update()
    {
        if (!isPlacing)
        {
            if (Input.GetMouseButtonDown(0))
            {
                StartPlacement();
            }
        }
        else
        {
            UpdatePreview();

            if (Input.GetMouseButtonDown(0))
            {
                PlaceBuilding();
            }

            if (Input.GetMouseButtonDown(1))
            {
                CancelPlacement();
            }
        }
    }

   void StartPlacement()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not assigned!");
            return;
        }

        int goldCost = database.GetGoldCostByID(objectIDToSpawn);

        if (GoldManager.Instance.CurrentGold < goldCost)
        {
            Debug.Log("Not enough gold to start placement.");
            return;
        }

        if (previewPrefab == null)
        {
            Debug.LogError("Preview Prefab not assigned!");
            return;
        }

        previewObject = Instantiate(previewPrefab);
        DisableColliders(previewObject);

        CalculatePreviewHeightOffset();

        isPlacing = true;
    }

    void UpdatePreview()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f))
        {
            if (hit.collider.GetComponent<Terrain>() == null)
                return;

            Vector3 alignedPos = hit.point;
            alignedPos.y += previewHeightOffset;

            previewObject.transform.position = alignedPos;
        }
    }

    void PlaceBuilding()
    {
        int goldCost = database.GetGoldCostByID(objectIDToSpawn);

        if (!GoldManager.Instance.SpendGold(goldCost))
        {
            Debug.Log("Not enough gold!");
            return;
        }

        Vector3 spawnPos = previewObject.transform.position;
        GameObject prefab = database.GetPrefabByID(objectIDToSpawn);

        Instantiate(prefab, spawnPos, Quaternion.identity);

        Destroy(previewObject);
        isPlacing = false;
    }

    void CancelPlacement()
    {
        Destroy(previewObject);
        isPlacing = false;
    }

    void CalculatePreviewHeightOffset()
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();

        if (renderers.Length == 0)
        {
            previewHeightOffset = 0f;
            return;
        }

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        previewHeightOffset = bounds.extents.y;
    }

    void DisableColliders(GameObject obj)
    {
        Collider[] colliders = obj.GetComponentsInChildren<Collider>();
        foreach (Collider c in colliders)
            c.enabled = false;
    }
}
