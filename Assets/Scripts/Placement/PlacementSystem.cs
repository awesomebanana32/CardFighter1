using UnityEngine;
using System.Collections.Generic;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectDatabaseSO database;
    [SerializeField] private ObjectPlacer objectPlacer;

    private IBuildingState buildingState;
    private GridData gridData;
    private Renderer previewRenderer;
    private Color validColor;

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
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        buildingState = new PlacementState(ID, grid, database, objectPlacer, gridData, cellIndicator, inputManager);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;
    }

    public void StartRemoving()
    {
        StopPlacement();
        buildingState = new RemovingState(grid, objectPlacer, gridData, cellIndicator, inputManager);
        inputManager.OnClicked += OnInputClicked;
        inputManager.OnExit += StopPlacement;
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
}