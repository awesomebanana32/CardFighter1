using UnityEngine;
using UnityEngine.UI;

public class ChaseButtonHandler : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Button chaseButton; // Assign your UI Button here

    [Header("Team Settings")]
    [Tooltip("Select the team this button controls.")]
    [SerializeField] private Team yourTeam; // The team that will respond to this button

    [Tooltip("Select one or more enemy teams to chase.")]
    [SerializeField] private Team[] targetTeams;

    private void Awake()
    {
        // Auto-assign button if not set
        if (chaseButton == null)
            chaseButton = GetComponent<Button>();

        if (chaseButton == null)
            Debug.LogError("ChaseButtonHandler: No Button component assigned or found!");
    }

    private void OnEnable()
    {
        if (chaseButton != null)
            chaseButton.onClick.AddListener(OnChaseButtonClicked);
    }

    private void OnDisable()
    {
        if (chaseButton != null)
            chaseButton.onClick.RemoveListener(OnChaseButtonClicked);
    }

    private void OnChaseButtonClicked()
    {
        if (targetTeams == null || targetTeams.Length == 0)
        {
            Debug.LogWarning("ChaseButtonHandler: No target teams selected.");
            return;
        }

        // Find all units in the scene
        StateManager[] allTroops = Object.FindObjectsOfType<StateManager>();

        foreach (StateManager troop in allTroops)
        {
            if (troop == null) continue;

            // Only affect units of "Your Team"
            if (troop.team != yourTeam) continue;

            // Command the unit to chase (ChaseState handles all enemies automatically)
            troop.CommandChase();
        }
    }
}
