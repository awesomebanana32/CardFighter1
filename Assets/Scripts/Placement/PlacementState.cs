using UnityEngine;

public class PlacementState : IBuildingState
{
    private int selectedObjectIndex = -1;
    private int ID;
    private Grid grid;
    private ObjectDatabaseSO database;
    private ObjectPlacer objectPlacer;
    private GridData gridData;
    private GameObject cellIndicator;
    private InputManager inputManager;
    private Vector3Int lastGridPosition;
    private Renderer cellIndicatorRenderer;
    private Color validColor;
    private Color invalidColor;
    private PlacementSystem placementSystem;
    private LayerMask unplaceableLayerMask; // Field to store the layer mask

    // Updated constructor to accept unplaceableLayerMask
    public PlacementState(int id, Grid grid, ObjectDatabaseSO database, ObjectPlacer objectPlacer, GridData gridData, GameObject cellIndicator, InputManager inputManager, PlacementSystem placementSystem, LayerMask unplaceableLayerMask)
    {
        this.ID = id;
        this.grid = grid;
        this.database = database;
        this.objectPlacer = objectPlacer;
        this.gridData = gridData;
        this.cellIndicator = cellIndicator;
        this.inputManager = inputManager;
        this.placementSystem = placementSystem;
        this.unplaceableLayerMask = unplaceableLayerMask; // Assign the layer mask

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == id);

        if (selectedObjectIndex == -1)
        {
            return;
        }

        if (cellIndicator != null)
        {
            cellIndicator.SetActive(true);
            cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
            ColorUtility.TryParseHtmlString("#A7FFA5", out validColor);
            ColorUtility.TryParseHtmlString("#FF4C4C", out invalidColor);
        }
    }

    public void EndState()
    {
        if (cellIndicator != null)
        {
            cellIndicator.SetActive(false);
        }
    }

    public void OnAction(Vector3Int gridPosition)
    {
        if (!CheckPlacementValidity(gridPosition, selectedObjectIndex))
        {
            return;
        }

        int cost = database.objectsData[selectedObjectIndex].PopulationCost;
        if (placementSystem.CurrentPopulation + cost > placementSystem.MaxPopulation)
        {
            return; // Cannot place due to population limit
        }

        int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
        if (index >= 0)
        {
            gridData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, index);
            placementSystem.AddToPopulation(cost); // Add to population after successful placement
            lastGridPosition = gridPosition;
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (cellIndicator == null || inputManager == null || cellIndicatorRenderer == null)
            return;

        cellIndicator.transform.position = grid.CellToWorld(gridPosition);

        bool gridValid = CheckPlacementValidity(gridPosition, selectedObjectIndex);
        int cost = database.objectsData[selectedObjectIndex].PopulationCost;
        bool populationValid = (placementSystem.CurrentPopulation + cost <= placementSystem.MaxPopulation);
        bool isValid = gridValid && populationValid;
        cellIndicatorRenderer.material.color = isValid ? validColor : invalidColor;

        if (gridPosition != lastGridPosition && Input.GetMouseButton(0) && !inputManager.IsPointerOverUI())
        {
            lastGridPosition = gridPosition;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !inputManager.IsPointerOverUI())
        {
            OnAction(gridPosition);
        }
    }

    private bool CheckPlacementValidity(Vector3Int gridPosition, int selectedObjectIndex)
    {
        if (selectedObjectIndex < 0 || selectedObjectIndex >= database.objectsData.Count)
            return false;

        bool isGridValid = gridData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
        if (!isGridValid)
            return false;

        Vector2Int objectSize = database.objectsData[selectedObjectIndex].Size;
        Vector3 worldPosition = grid.CellToWorld(gridPosition);
        Vector3 boxSize = new Vector3(objectSize.x * grid.cellSize.x, 1f, objectSize.y * grid.cellSize.z) * 0.5f;
        Vector3 boxCenter = worldPosition + new Vector3(boxSize.x, 0.5f, boxSize.z);

        Collider[] colliders = Physics.OverlapBox(boxCenter, boxSize, Quaternion.identity);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Unplaceable"))
            {
                return false;
            }
        }

        return true;
    }
}