using UnityEngine;
using System.Collections.Generic;

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

    public int MaxPopulation => maxPopulation;
    public int CurrentPopulation => currentPopulation;

    public void AddToPopulation(int amount)
    {
        currentPopulation += amount;
        if (currentPopulation < 0) currentPopulation = 0;
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
        // Initialize unplaceable objects' meshes as invisible
        ToggleUnplaceableMeshes(false);
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID, grid, database, objectPlacer, gridData, cellIndicator, inputManager, this, unplaceableLayerMask);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;

        // Show all unplaceable objects' meshes
        ToggleUnplaceableMeshes(true);
    }

    public void StartRemoving()
    {
        StopPlacement();
        buildingState = new RemovingState(grid, objectPlacer, gridData, cellIndicator, inputManager, database, this);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;

        // Show all unplaceable objects' meshes
        ToggleUnplaceableMeshes(true);
    }

    private void OnInputClicked()
    {
        if (buildingState == null || inputManager.IsPointerOverUI())
            return;

        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState.OnAction(gridPosition);
        }
    }

    private void StopPlacement()
    {
        if (buildingState == null)
            return;

        buildingState.EndState();
        inputManager.OnClicked -= OnInputClicked;
        inputManager.OnExit -= StopPlacement;
        buildingState = null;

        // Hide all unplaceable objects' meshes
        ToggleUnplaceableMeshes(false);
    }

    private void Update()
    {
        if (buildingState == null || inputManager == null)
            return;

        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            buildingState.UpdateState(gridPosition);
        }
    }

    private void ToggleUnplaceableMeshes(bool visible)
    {
        // Find all colliders in the unplaceableLayerMask
        Collider[] unplaceableColliders = Physics.OverlapSphere(Vector3.zero, float.MaxValue, unplaceableLayerMask);
        foreach (var collider in unplaceableColliders)
        {
            // Optionally check for a specific tag
            if (collider.CompareTag("Unplaceable"))
            {
                // Get all Renderer components (e.g., MeshRenderer) on the GameObject
                Renderer[] renderers = collider.gameObject.GetComponentsInChildren<Renderer>();
                foreach (var renderer in renderers)
                {
                    renderer.enabled = visible;
                }
            }
        }
    }
}