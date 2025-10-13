using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class SelectionManager : MonoBehaviour
{
    private Transform selectedTroop;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private string playerTroopTag = "TeamGreen";

    void Update()
    {
        if (Mouse.current == null || Camera.main == null)
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ~0))
            {
                if (hit.transform.CompareTag(playerTroopTag))
                {
                    selectedTroop = hit.transform;
                    Debug.Log($"Troop selected: {selectedTroop.name}");
                }
                else if (selectedTroop != null && ((1 << hit.collider.gameObject.layer) & groundLayer) != 0)
                {
                    StateManager stateManager = selectedTroop.GetComponent<StateManager>();
                    if (stateManager != null)
                    {
                        stateManager.CommandMove(hit.point);
                        Debug.Log($"Ground selected for move command at: {hit.point}");
                    }

                    selectedTroop = null;
                }
            }
        }
    }
}
