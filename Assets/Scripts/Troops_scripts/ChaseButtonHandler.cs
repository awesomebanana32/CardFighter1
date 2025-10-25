using UnityEngine;
using UnityEngine.UI;

public class ChaseButtonHandler : MonoBehaviour
{
    [SerializeField] private string enemyTag = "TeamGreen"; // Set to match enemy team
    [SerializeField] private Button chaseButton; // Reference to the UI button

    private void Awake()
    {
        // Ensure button is assigned
        if (chaseButton == null)
        {
            chaseButton = GetComponent<Button>();
            if (chaseButton == null)
            {
                //Debug.LogError("ChaseButtonHandler: No Button component assigned or found!");
            }
        }
    }

    private void OnEnable()
    {
        if (chaseButton != null)
        {
            chaseButton.onClick.AddListener(OnChaseButtonClicked);
        }
    }

    private void OnDisable()
    {
        if (chaseButton != null)
        {
            chaseButton.onClick.RemoveListener(OnChaseButtonClicked);
        }
    }

    private void OnChaseButtonClicked()
    {
        // Call the static method to command all troops to chase
        StateManager.CommandAllToChase(enemyTag);
    }
}