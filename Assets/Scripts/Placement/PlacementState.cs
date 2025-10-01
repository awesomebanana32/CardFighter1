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

    public PlacementState(int id, Grid grid, ObjectDatabaseSO database, ObjectPlacer objectPlacer, GridData gridData, GameObject cellIndicator, InputManager inputManager)
    {
        this.ID = id;
        this.grid = grid;
        this.database = database;
        this.objectPlacer = objectPlacer;
        this.gridData = gridData;
        this.cellIndicator = cellIndicator;
        this.inputManager = inputManager;

        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == id);

        if (selectedObjectIndex == -1)
        {
            Debug.LogError($"Object with ID {id} not found in database.");
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
        if (CheckPlacementValidity(gridPosition, selectedObjectIndex))
        {
            int index = objectPlacer.PlaceObject(database.objectsData[selectedObjectIndex].Prefab, grid.CellToWorld(gridPosition));
            if (index >= 0)
            {
                gridData.AddObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size, database.objectsData[selectedObjectIndex].ID, index);
                lastGridPosition = gridPosition;
            }
        }
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (cellIndicator == null || inputManager == null || cellIndicatorRenderer == null)
            return;

        cellIndicator.transform.position = grid.CellToWorld(gridPosition);

        bool isValid = CheckPlacementValidity(gridPosition, selectedObjectIndex);
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
        return gridData.CanPlaceObjectAt(gridPosition, database.objectsData[selectedObjectIndex].Size);
    }
}