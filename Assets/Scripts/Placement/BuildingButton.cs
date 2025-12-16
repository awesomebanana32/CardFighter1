using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    [Header("Database")]
    public ObjectDatabaseSO database;

    [Header("Gameplay")]
    public int objectIDToSpawn = 0;
    public Camera mainCamera;
    public float cityRadius = 20f;           
    public LayerMask cityLayer;              // Layer for cities
    public LayerMask buildingLayer;          // Layer for existing buildings

    [Header("Preview")]
    public GameObject previewPrefab;

    private GameObject previewObject;
    private bool isPlacing = false;
    private float previewHeightOffset;
    private bool canPlaceHere = false;

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

            if (Input.GetMouseButtonDown(0) && canPlaceHere)
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

            // Check if placement is valid
            bool withinCity = IsWithinCityRadius(alignedPos);
            bool overlapping = IsOverlappingBuilding(alignedPos);

            canPlaceHere = withinCity && !overlapping;

            SetPreviewColor(canPlaceHere);
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

    bool IsWithinCityRadius(Vector3 position)
    {
        Collider[] hits = Physics.OverlapSphere(position, cityRadius, cityLayer);
        return hits.Length > 0;
    }

    bool IsOverlappingBuilding(Vector3 position)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return false;

        // Calculate bounds for placement check
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
        {
            bounds.Encapsulate(renderers[i].bounds);
        }

        // Check for overlaps with building layer
        Collider[] hits = Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, buildingLayer);
        return hits.Length > 0;
    }

    void SetPreviewColor(bool valid)
    {
        if (previewObject == null) return;

        Color color = valid ? Color.green : Color.red;

        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer r in renderers)
        {
            foreach (Material m in r.materials)
                m.color = color;
        }
    }
}
