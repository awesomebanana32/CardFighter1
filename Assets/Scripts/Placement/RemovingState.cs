using UnityEngine;

public class RemovingState : IBuildingState
{
    private int gameObjectIndex = -1;
    private Grid grid;
    private ObjectPlacer objectPlacer;
    private GridData gridData;
    private GameObject cellIndicator;
    private InputManager inputManager;
    private Renderer cellIndicatorRenderer;
    private Color validColor;
    private Color invalidColor;
    private Vector3Int lastGridPosition;

    public RemovingState(Grid grid, ObjectPlacer objectPlacer, GridData gridData, GameObject cellIndicator, InputManager inputManager)
    {
        this.grid = grid;
        this.objectPlacer = objectPlacer;
        this.gridData = gridData;
        this.cellIndicator = cellIndicator;
        this.inputManager = inputManager;

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
        if (!gridData.GetRepresentationIndex(gridPosition, out gameObjectIndex))
        {
            // Add sound or feedback for invalid removal attempt later
            return;
        }

        gridData.RemoveObjectAt(gridPosition);
        objectPlacer.RemoveObjectAt(gameObjectIndex);
    }

    public void UpdateState(Vector3Int gridPosition)
    {
        if (cellIndicator == null || cellIndicatorRenderer == null || inputManager == null)
            return;

        cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        bool canRemove = gridData.GetRepresentationIndex(gridPosition, out _);
        cellIndicatorRenderer.material.color = canRemove ? validColor : invalidColor;

        if (gridPosition != lastGridPosition && Input.GetMouseButton(0) && !inputManager.IsPointerOverUI())
        {
            OnAction(gridPosition);
            lastGridPosition = gridPosition;
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)) && !inputManager.IsPointerOverUI())
        {
            OnAction(gridPosition);
            lastGridPosition = gridPosition;
        }
    }
}