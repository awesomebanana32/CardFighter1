using UnityEngine;
using UnityEngine.UI;

public class LightningSpawner : MonoBehaviour
{
    [Header("Assign in Inspector")]
    [SerializeField] private GameObject lightningPrefab;
    [SerializeField] private Camera mainCamera;

    [Header("Cooldown Settings")]
    [SerializeField] private float cooldownTime = 10f;

    [Header("UI Settings")]
    [SerializeField] private Button castButton;
    [SerializeField] private Image cooldownOverlay;

    [Header("Gold Settings")]
    [SerializeField] private int lightningCost = 50;

    private bool lightningModeActive = false;
    private bool canCast = true;
    private float cooldownTimer = 0f;

    private GoldManager gold;

    private void Start()
    {
        gold = GoldManager.Instance;
    }

    public void EnableLightningMode()
    {
        if (!canCast)
        {
            Debug.Log("Lightning spell is on cooldown!");
            return;
        }

        // Try to spend gold now, only allow mode if successful
        if (!gold.SpendGold(lightningCost))
        {
            Debug.Log("Not enough gold to cast lightning!");
            return;
        }

        lightningModeActive = true;
    }

    private void Update()
    {
        HandleCooldown();

        if (!lightningModeActive || !canCast)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                SpawnLightning(hit.point);
                StartCooldown();
            }

            lightningModeActive = false;
        }
    }

    private void SpawnLightning(Vector3 position)
    {
        if (lightningPrefab != null)
            Instantiate(lightningPrefab, position, Quaternion.identity);
    }

    private void StartCooldown()
    {
        canCast = false;
        cooldownTimer = cooldownTime;

        if (castButton != null)
            castButton.interactable = false;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;
    }

    private void HandleCooldown()
    {
        if (!canCast)
        {
            cooldownTimer -= Time.deltaTime;

            if (cooldownOverlay != null)
                cooldownOverlay.fillAmount = cooldownTimer / cooldownTime;

            if (cooldownTimer <= 0f)
            {
                canCast = true;
                if (castButton != null)
                    castButton.interactable = true;
                if (cooldownOverlay != null)
                    cooldownOverlay.fillAmount = 0f;
            }
        }
    }
}
