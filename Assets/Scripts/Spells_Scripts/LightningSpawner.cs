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
    [SerializeField] private Button castButton;       // assign your lightning button here
    [SerializeField] private Image cooldownOverlay;   // assign overlay image (child of button)

    private bool lightningModeActive = false;
    private bool canCast = true;
    private float cooldownTimer = 0f;

    public void EnableLightningMode()
    {
        if (!canCast)
        {
            Debug.Log("Lightning spell is on cooldown!");
            return;
        }

        lightningModeActive = true;
        Debug.Log("Lightning mode enabled. Right-click to cast.");
    }

    private void Update()
    {
        // --- Cooldown update ---
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

        // --- Lightning casting ---
        if (!lightningModeActive || !canCast) return;

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
        {
            Instantiate(lightningPrefab, position, Quaternion.identity);
            Debug.Log($"Lightning spawned at {position}");
        }
        else
        {
            Debug.LogWarning("Lightning prefab not assigned!");
        }
    }

    private void StartCooldown()
    {
        canCast = false;
        cooldownTimer = cooldownTime;

        if (castButton != null)
            castButton.interactable = false;

        if (cooldownOverlay != null)
            cooldownOverlay.fillAmount = 1f;

        Debug.Log("Lightning cooldown started.");
    }
}
