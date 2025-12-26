using UnityEngine;

public class BuildingButton : MonoBehaviour
{
    [Header("Database")]
    public ObjectDatabaseSO database;

    [Header("Gameplay")]
    public int objectIDToSpawn = 0;
    public Camera mainCamera;
    public float cityRadius = 20f;
    public LayerMask cityLayer;
    public LayerMask buildingLayer;

    [Header("Preview")]
    public GameObject previewPrefab;

    private GameObject previewObject;
    private bool isPlacing = false;
    private float previewHeightOffset;
    private bool canPlaceHere = false;

    void Update()
    {
        if (!isPlacing)
            return;

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

    // Called only by UI button
    public void StartPlacement()
    {
        if (isPlacing)
            return;

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

        if (!Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("ground")))
            return;

        Vector3 alignedPos = hit.point;
        alignedPos.y += previewHeightOffset;
        previewObject.transform.position = alignedPos;

        bool withinCity = IsWithinCityRadius(alignedPos);
        bool overlapping = IsOverlappingBuilding();

        canPlaceHere = withinCity && !overlapping;
        SetPreviewColor(canPlaceHere);
    }

    void PlaceBuilding()
    {
        int goldCost = database.GetGoldCostByID(objectIDToSpawn);

        if (!GoldManager.Instance.SpendGold(goldCost))
            return;

        GameObject prefab = database.GetPrefabByID(objectIDToSpawn);
        Instantiate(prefab, previewObject.transform.position, Quaternion.identity);

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
            bounds.Encapsulate(renderers[i].bounds);

        previewHeightOffset = bounds.extents.y;
    }

    void DisableColliders(GameObject obj)
    {
        foreach (Collider c in obj.GetComponentsInChildren<Collider>())
            c.enabled = false;
    }

    bool IsWithinCityRadius(Vector3 position)
    {
        return Physics.OverlapSphere(position, cityRadius, cityLayer).Length > 0;
    }

    bool IsOverlappingBuilding()
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0)
            return false;

        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++)
            bounds.Encapsulate(renderers[i].bounds);

        return Physics.OverlapBox(bounds.center, bounds.extents, Quaternion.identity, buildingLayer).Length > 0;
    }

    void SetPreviewColor(bool valid)
    {
        Color color = valid ? Color.green : Color.red;

        foreach (Renderer r in previewObject.GetComponentsInChildren<Renderer>())
        {
            foreach (Material m in r.materials)
                m.color = color;
        }
    }
}
