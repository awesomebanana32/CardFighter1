using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(Button))]
public class TroopButton : MonoBehaviour
{
    public int troopID;

    [Header("UI References")]
    public Image cooldownImage;      // Radial overlay image (Image.Type = Filled, Fill Method = Radial360)
    public TMP_Text queueCountText;  // TextMeshProUGUI showing queued count

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClick);

        if (cooldownImage != null)
            cooldownImage.fillAmount = 0f;

        UpdateQueueCount(0);
    }

    private void OnDestroy()
    {
        if (btn != null)
            btn.onClick.RemoveListener(OnClick);
    }

    public void OnClick()
    {
        if (PlacementSystem.Instance != null)
            PlacementSystem.Instance.QueueSpawn(troopID, cooldownImage);
        else
            Debug.LogWarning("PlacementSystem instance not found.");
    }

    // Called by PlacementSystem via RefreshButtonQueueDisplay
    public void UpdateQueueCount(int count)
    {
        if (queueCountText == null) return;
        queueCountText.text = (count > 0) ? count.ToString() : "";
    }
}
