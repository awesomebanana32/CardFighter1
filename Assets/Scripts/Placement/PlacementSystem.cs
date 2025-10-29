using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectDatabaseSO database;
    [SerializeField] private ObjectPlacer objectPlacer;
    [SerializeField] private int maxPopulation = 100;
    [SerializeField] private LayerMask unplaceableLayerMask;

    private IBuildingState buildingState;
    private GridData gridData;
    private Renderer previewRenderer;
    private Color validColor;
    private int currentPopulation = 0;

    // Singleton instance
    public static PlacementSystem Instance { get; private set; }

    public int MaxPopulation => maxPopulation;
    public int CurrentPopulation => currentPopulation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        StopPlacement();
        gridData = new GridData();
        previewRenderer = cellIndicator?.GetComponentInChildren<Renderer>();
        if (previewRenderer != null)
        {
            ColorUtility.TryParseHtmlString("#A7FFA5", out validColor);
            previewRenderer.material.color = validColor;
        }

        ToggleUnplaceableMeshes(false);
    }

    /// <summary>
    /// Safely adds population, clamped to [0, maxPopulation]
    /// </summary>
    public void AddToPopulation(int amount)
    {
        currentPopulation += amount;
        currentPopulation = Mathf.Clamp(currentPopulation, 0, maxPopulation);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID, grid, database, objectPlacer, gridData, cellIndicator, inputManager, this, unplaceableLayerMask);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;

        ToggleUnplaceableMeshes(true);
    }

    public void StartRemoving()
    {
        StopPlacement();
        buildingState = new RemovingState(grid, objectPlacer, gridData, cellIndicator, inputManager, database, this);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;

        ToggleUnplaceableMeshes(true);
    }

    private void OnInputClicked()
    {
        if (buildingState == null || inputManager == null || inputManager.IsPointerOverUI())
            return;

        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState?.OnAction(gridPosition);
        }
    }

    public void StopPlacementButton() => StopPlacement();

    private void StopPlacement()
    {
        if (buildingState == null) return;

        buildingState.EndState();
        inputManager.OnClicked -= OnInputClicked;
        inputManager.OnExit -= StopPlacement;
        buildingState = null;

        ToggleUnplaceableMeshes(false);
    }

    private void Update()
    {
        if (buildingState == null || inputManager == null) return;

        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState?.UpdateState(gridPosition);
        }
    }

    private void ToggleUnplaceableMeshes(bool visible)
    {
        Collider[] unplaceableColliders = Physics.OverlapSphere(Vector3.zero, float.MaxValue, unplaceableLayerMask);
        foreach (var collider in unplaceableColliders)
        {
            if (collider.CompareTag("Unplaceable"))
            {
                Renderer[] renderers = collider.gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = visible;
                }
            }
        }
    }
}
