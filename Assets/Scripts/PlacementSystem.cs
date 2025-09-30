using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject mouseIndicator; // Fixed: Separate declaration
    [SerializeField]
    private GameObject cellIndicator; // Fixed: Added type and removed stray semicolon
    [SerializeField]
    private InputManager inputManager;
    [SerializeField]
    private Grid grid;

    private void Update()
    {
        if (inputManager.GetSelectedMapPosition(out Vector3 mousePosition))
        {
            mouseIndicator.transform.position = mousePosition;
            Vector3Int gridPosition = grid.WorldToCell(mousePosition); 
            cellIndicator.transform.position = grid.CellToWorld(gridPosition); // Fixed: Correct method name
        }
    }
}