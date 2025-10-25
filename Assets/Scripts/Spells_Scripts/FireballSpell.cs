using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class FireballSpell : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 10f;
    [SerializeField] private float spawnOffset = 0.5f; // prevent collision with caster
    [SerializeField] private LayerMask raycastLayers = ~0; // default: hit everything

    private Camera mainCamera;
    private bool isCasting = false;

    private void Awake()
    {
        mainCamera = Camera.main;
        if (mainCamera == null)
            Debug.LogError("Main camera not found!");
    }

    // Called by UI button to enter casting mode
    public void StartCasting()
    {
        isCasting = true;
        Debug.Log("Fireball casting mode enabled! Right-click to cast.");
    }

    private void Update()
    {
        if (!isCasting || Mouse.current == null)
            return;

        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Mouse.current.position.ReadValue();
            Debug.Log($"Right-click detected at screen position: {mousePos}");

            Ray ray = mainCamera.ScreenPointToRay(mousePos);

            // Draw the ray in Scene view for 1 second
            Debug.DrawRay(ray.origin, ray.direction * 100f, Color.red, 1f);

            // --- Optional: check if pointer is over UI ---
            // if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            // {
            //     Debug.Log("Click blocked by UI element.");
            //     return;
            // }

            if (Physics.Raycast(ray, out RaycastHit hit, 1000f, raycastLayers, QueryTriggerInteraction.Ignore))
            {
                string hitLayer = LayerMask.LayerToName(hit.collider.gameObject.layer);
                Debug.Log($"Raycast hit: {hit.collider.name} (Layer: {hitLayer}) at {hit.point}");

                Vector3 direction = (hit.point - transform.position).normalized;
                Vector3 spawnPos = transform.position + direction * spawnOffset;

                SpawnFireball(spawnPos, direction);
            }
            else
            {
                Debug.Log("Raycast hit nothing, shooting forward.");
                Vector3 direction = ray.direction;
                Vector3 spawnPos = transform.position + direction * spawnOffset;

                SpawnFireball(spawnPos, direction);
            }

            isCasting = false;
        }
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
}
