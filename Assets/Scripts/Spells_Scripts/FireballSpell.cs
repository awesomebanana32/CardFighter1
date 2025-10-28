using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class FireballSpell : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private float spawnOffset = 0.5f;
    [SerializeField] private LayerMask raycastLayers = ~0;
    [SerializeField] private float cooldownTime = 10f; // cooldown in seconds

    [Header("UI Settings")]
    [SerializeField] private Button castButton; // assign in inspector
    [SerializeField] private Image cooldownOverlay; // assign a child UI image

    private Camera mainCamera;
    private bool isCasting = false;
    private bool canCast = true;
    private float cooldownTimer = 0f;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main camera not found!");
    }

    private void Update()
    {
        // --- Handle cooldown UI ---
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

        // --- Fireball casting logic ---
        if (!isCasting || Mouse.current == null || !canCast)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Debug.Log($"Right-click detected at screen position: {mousePos}");

            Ray ray = mainCamera.ScreenPointToRay(mousePos);
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, raycastLayers, QueryTriggerInteraction.Ignore))
            {
                Vector3 direction = (hit.point - transform.position).normalized;
                Vector3 spawnPos = transform.position + direction * spawnOffset;
                SpawnFireball(spawnPos, direction);
            }
            else
            {
                Vector3 direction = ray.direction;
                Vector3 spawnPos = transform.position + direction * spawnOffset;
                SpawnFireball(spawnPos, direction);
            }

            StartCooldown();
            isCasting = false;
        }
    }

    public void StartCasting()
    {
        if (!canCast)
        {
            Debug.Log("Spell is on cooldown!");
            return;
        }

        isCasting = true;
        Debug.Log("Fireball casting mode enabled! Right-click to cast.");
    }

    private void SpawnFireball(Vector3 position, Vector3 direction)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("Fireball prefab not assigned!");
            return;
        }

        GameObject fireball = Instantiate(fireballPrefab, position, Quaternion.LookRotation(direction));

        if (fireball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;
            rb.linearVelocity = direction * fireballSpeed;
            Debug.Log($"Fireball spawned at {position}, moving toward {direction}");
        }
        else
        {
            Debug.LogWarning("Fireball prefab has no Rigidbody!");
        }
    }

    private void StartCooldown()
    {
        canCast = false;
        cooldownTimer = cooldownTime;

        if (castButton != null)
            castButton.interactable = false;

        if (cooldownOverlay != null)
        {
            cooldownOverlay.fillAmount = 1f;
        }

        Debug.Log("Fireball spell on cooldown.");
    }
}
