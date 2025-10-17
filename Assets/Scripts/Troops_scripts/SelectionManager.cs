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
    private List<Transform> selectedTroops = new List<Transform>();
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
            isDragging = false;
            selectedTroops.Clear();
        }

        Debug.Log($"Selection mode {(isSelectionActive ? "ACTIVATED" : "DEACTIVATED")}");
    }

    void Update()
    {
        if (!isSelectionActive || Mouse.current == null || Camera.main == null) return;

        // Clean out any destroyed troops
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
            selectedTroops.Clear();
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
                selectedTroops.Clear();
                selectedTroops.Add(hitObject.transform);
                Debug.Log($"Single troop selected: {hitObject.name}");
            }
            else
            {
                Debug.Log("Hit object is not a player troop.");
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
                selectedTroops.Add(troop.transform);
                Debug.Log($"Troop selected: {troop.name}");
            }
        }
    }

    private void HandleMoveCommand()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame && selectedTroops.Count > 0 && !EventSystem.current.IsPointerOverGameObject())
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                // Send move command to all valid troops
                foreach (Transform troop in selectedTroops)
                {
                    if (troop == null) continue;

                    StateManager stateManager = troop.GetComponent<StateManager>();
                    if (stateManager != null)
                    {
                        stateManager.CommandMove(hit.point);
                    }
                }

                // Deselect after command
                selectedTroops.Clear();
                Debug.Log("Troops deselected after move command.");
            }
        }
    }

    private void CleanupDestroyedTroops()
    {
        // Remove destroyed or missing troops safely
        for (int i = selectedTroops.Count - 1; i >= 0; i--)
        {
            if (selectedTroops[i] == null)
            {
                selectedTroops.RemoveAt(i);
            }
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
