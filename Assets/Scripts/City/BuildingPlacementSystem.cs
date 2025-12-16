using UnityEngine;

public class BuildingPlacementSystem : MonoBehaviour
{
    public static BuildingPlacementSystem Instance { get; private set; }

    [Header("Layers")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask buildingLayer;

    [Header("Placement Settings")]
    [SerializeField] private float buildingCheckRadius = 1.5f;

    private GameObject buildingPrefab;
    private SpawnRadius selectedSpawnRadius;
    private bool isPlacing;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Update()
    {
        if (!isPlacing) return;

        // Left click = try to place
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Left click detected, trying to place building...");
            TryPlaceBuilding();
        }

        // Right click = cancel placement
        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Right click detected, canceling placement");
            CancelPlacement();
        }
    }

    /// <summary>
    /// Called by UI button to start placement
    /// </summary>
    public void StartPlacingBuilding(GameObject prefab)
    {
        Debug.Log("StartPlacingBuilding called");

        City selectedCity = CitySelectionManager.SelectedCity;
        if (selectedCity == null)
        {
            Debug.LogWarning("No city selected!");
            return;
        }

        selectedSpawnRadius = selectedCity.GetComponent<SpawnRadius>();
        if (selectedSpawnRadius == null)
        {
            Debug.LogWarning("Selected city does not have a SpawnRadius component!");
            return;
        }

        buildingPrefab = prefab;
        isPlacing = true;

        Debug.Log("Placement started for prefab: " + buildingPrefab.name + " in city: " + selectedCity.name);
    }

    private void TryPlaceBuilding()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            Debug.Log("Raycast did not hit ground");
            return;
        }

        Vector3 point = hit.point;
        Debug.Log("Raycast hit point: " + point);

        // Check if inside city radius
        if (!selectedSpawnRadius.IsPointInsideRadius(point))
        {
            Debug.Log("Point is outside spawn radius");
            return;
        }
        else
        {
            Debug.Log("Point is inside spawn radius");
        }

        // Check if overlapping other buildings
        if (IsBuildingBlocking(point))
        {
            Debug.Log("Point blocked by existing building");
            return;
        }
        else
        {
            Debug.Log("No building overlap detected, placement valid");
        }

        PlaceBuilding(point);
    }

    private bool IsBuildingBlocking(Vector3 point)
    {
        Collider[] hits = Physics.OverlapSphere(point, buildingCheckRadius, buildingLayer);
        if (hits.Length > 0)
        {
            foreach (Collider c in hits)
            {
                Debug.Log("Blocking building detected: " + c.name);
            }
            return true;
        }
        return false;
    }

    private void PlaceBuilding(Vector3 point)
    {
        Debug.Log("Placing building at: " + point);
        Instantiate(buildingPrefab, point, Quaternion.identity);
        CancelPlacement();
    }

    private void CancelPlacement()
    {
        Debug.Log("Cancelling building placement");
        isPlacing = false;
        buildingPrefab = null;
        selectedSpawnRadius = null;
    }
}
