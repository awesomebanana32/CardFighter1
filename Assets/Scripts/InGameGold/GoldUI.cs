using UnityEngine;
using TMPro;

public class GoldUI : MonoBehaviour
{
    [SerializeField] private TMP_Text goldText;

    void Start()
    {
        // Subscribe after GoldManager.Awake has run
        GoldManager.Instance.OnGoldChanged += UpdateGoldText;

        // Set initial label
        UpdateGoldText(GoldManager.Instance.CurrentGold);
    }

    private void OnDestroy()
    {
        // Unsubscribe (prevents errors when leaving the scene)
        if (GoldManager.Instance != null)
            GoldManager.Instance.OnGoldChanged -= UpdateGoldText;
    }

    void UpdateGoldText(int newAmount)
    {
        goldText.text = "Gold: " + newAmount;
    }
}
