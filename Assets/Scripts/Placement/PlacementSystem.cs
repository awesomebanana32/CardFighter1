using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private GameObject mouseIndicator;
    [SerializeField] private GameObject cellIndicator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private Grid grid;
    [SerializeField] private ObjectDatabaseSO database;
    private int selectedObjectIndex = -1;

    private void Start()
    {
        StopPlacement();
    }

    public void StartPlacement(int ID)
    {
        StopPlacement();
        // Use the correct field name from your ObjectDatabaseSO
        selectedObjectIndex = database.objectsData.FindIndex(data => data.ID == ID);
        if (selectedObjectIndex < 0)
        {
            Debug.LogError($"No ID found {ID}");
            return;
        }
        cellIndicator.SetActive(true);
        // Subscribe to events using instance reference
        inputManager.OnClicked += PlaceStructure;
        inputManager.OnExit += StopPlacement;
    }

    private void PlaceStructure()
    {
        if (inputManager.IsPointerOverUI())
            return;
        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            // Instantiate using the correct list
            GameObject newObject = Instantiate(database.objectsData[selectedObjectIndex].Prefab);
            newObject.transform.position = grid.CellToWorld(gridPosition);
        }
    }

    private void StopPlacement()
    {
        selectedObjectIndex = -1;
        cellIndicator.SetActive(false);
        inputManager.OnClicked -= PlaceStructure;
        inputManager.OnExit -= StopPlacement;
    }

    private void Update()
    {
        if (selectedObjectIndex < 0)
            return;
        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            mouseIndicator.transform.position = mousePosition;
            Vector3Int gridPosition = grid.WorldToCell(mousePosition);
            cellIndicator.transform.position = grid.CellToWorld(gridPosition);
        }
    }
}