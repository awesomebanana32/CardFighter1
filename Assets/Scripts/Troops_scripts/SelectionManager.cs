using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class SelectionManager : MonoBehaviour
{
    [Header("Troop Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask troopLayer;
    [SerializeField] private string playerTroopTag = "TeamGreen";

    [Header("UI")]
    [SerializeField] private Button selectionModeButton;

    private bool isSelectionActive = false;
    private List<CircleSelection> selectedUnits = new List<CircleSelection>();
    private Vector2 mouseStartPos;
    private bool isDragging = false;
    private Rect selectionRect;

    void Start()
    {
        if (selectionModeButton != null)
        {
            selectionModeButton.onClick.AddListener(ToggleSelectionMode);
        }
        else
        {
            Debug.LogWarning("SelectionModeButton not assigned in inspector!");
        }
    }

    void OnDestroy()
    {
        if (selectionModeButton != null)
            selectionModeButton.onClick.RemoveListener(ToggleSelectionMode);
    }

    private void ToggleSelectionMode()
    {
        isSelectionActive = !isSelectionActive;

        if (selectionModeButton != null)
        {
            ColorBlock colors = selectionModeButton.colors;
            colors.normalColor = isSelectionActive ? Color.green : Color.white;
            selectionModeButton.colors = colors;
        }

        if (!isSelectionActive)
        {
            DeselectAll(); 
            isDragging = false;
        }

        Debug.Log($"Selection mode {(isSelectionActive ? "ACTIVATED" : "DEACTIVATED")}");
    }

    void Update()
    {
        if (!isSelectionActive || Mouse.current == null || Camera.main == null) return;

        CleanupDestroyedTroops();
        HandleSelection();
        HandleMoveCommand();
    }

    private void HandleSelection()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject())
        {
            mouseStartPos = Mouse.current.position.ReadValue();
            isDragging = true;
            DeselectAll(); 
        }

        if (isDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            selectionRect = new Rect(
                Mathf.Min(mouseStartPos.x, currentMousePos.x),
                Mathf.Min(mouseStartPos.y, currentMousePos.y),
                Mathf.Abs(currentMousePos.x - mouseStartPos.x),
                Mathf.Abs(currentMousePos.y - mouseStartPos.y)
            );

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                if (selectionRect.width < 5f && selectionRect.height < 5f)
                    SingleClickSelect();
                else
                    DragSelect();

                isDragging = false;
            }
        }
    }

    private void SingleClickSelect()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 2f);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            GameObject hitObject = hit.collider.gameObject;

            if (hitObject.CompareTag(playerTroopTag))
            {
                DeselectAll();

                CircleSelection unit = hitObject.GetComponent<CircleSelection>();
                if (unit != null)
                {
                    selectedUnits.Add(unit);
                    unit.Select();
                }

                Debug.Log($"Single troop selected: {hitObject.name}");
            }
        }
    }

    private void DragSelect()
    {
        GameObject[] troops = GameObject.FindGameObjectsWithTag(playerTroopTag);
        foreach (GameObject troop in troops)
        {
            if (troop == null) continue;

            Vector3 screenPos = Camera.main.WorldToScreenPoint(troop.transform.position);
            if (selectionRect.Contains(new Vector2(screenPos.x, screenPos.y)))
            {
                CircleSelection unit = troop.GetComponent<CircleSelection>();
                if (unit != null)
                {
                    selectedUnits.Add(unit);
                    unit.Select(); 
                }

                Debug.Log($"Troop selected: {troop.name}");
            }
        }
    }

   private void HandleMoveCommand()
{
    if (Mouse.current.rightButton.wasPressedThisFrame && selectedUnits.Count > 0 && !EventSystem.current.IsPointerOverGameObject())
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
        {
            Vector3 baseDestination = hit.point;
            int unitCount = selectedUnits.Count;

            // Formation settings
            int columns = Mathf.CeilToInt(Mathf.Sqrt(unitCount)); // auto-calculate formation width
            float spacing = 3.0f; // distance between troops
            Vector3 forward = Vector3.zero;
            Vector3 right = Vector3.zero;

            // Align the formation to the camera direction (so "rows" face the player)
            if (Camera.main != null)
            {
                forward = Camera.main.transform.forward;
                forward.y = 0;
                forward.Normalize();

                right = Camera.main.transform.right;
                right.y = 0;
                right.Normalize();
            }
            else
            {
                forward = Vector3.forward;
                right = Vector3.right;
            }

            // Calculate the total number of rows
            int rows = Mathf.CeilToInt((float)unitCount / columns);

            // Find the top-left corner so the formation is centered on the click
            Vector3 formationCenterOffset =
                (-right * (columns - 1) * spacing / 2f) +
                (forward * (rows - 1) * spacing / 2f);

            for (int i = 0; i < unitCount; i++)
            {
                if (selectedUnits[i] == null) continue;

                int row = i / columns;
                int column = i % columns;

                Vector3 offset = (right * (column * spacing)) - (forward * (row * spacing));
                Vector3 targetPosition = baseDestination + formationCenterOffset + offset;

                StateManager stateManager = selectedUnits[i].GetComponent<StateManager>();
                if (stateManager != null)
                    stateManager.CommandMove(targetPosition);

                selectedUnits[i].Deselect();
            }

            selectedUnits.Clear();
            Debug.Log($"Troops moved in a {rows}x{columns} formation and deselected.");
        }
    }
}

    private void DeselectAll()
    {
        foreach (CircleSelection unit in selectedUnits)
        {
            if (unit != null)
                unit.Deselect();
        }
        selectedUnits.Clear();
    }

    private void CleanupDestroyedTroops()
    {
        for (int i = selectedUnits.Count - 1; i >= 0; i--)
        {
            if (selectedUnits[i] == null)
                selectedUnits.RemoveAt(i);
        }
    }

    void OnGUI()
    {
        if (isDragging)
        {
            float screenHeight = Screen.height;
            Rect guiRect = new Rect(
                selectionRect.x,
                screenHeight - (selectionRect.y + selectionRect.height),
                selectionRect.width,
                selectionRect.height
            );

            GUI.color = new Color(0, 1, 0, 0.2f);
            GUI.DrawTexture(guiRect, Texture2D.whiteTexture);
            GUI.color = Color.white;
        }
    }
}
