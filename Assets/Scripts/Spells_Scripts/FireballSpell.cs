using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class FireballSpell : MonoBehaviour
{
    [Header("Spell Settings")]
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private float fireballSpeed = 10f;
    [Header("Optional: Only hits these layers, leave empty to hit all")]
    [SerializeField] private LayerMask groundLayer;

    private Camera mainCamera;
    private bool isCasting = false; // Tracks if player is in casting mode

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Called by left-click on UI button
    public void StartCasting()
    {
        isCasting = true;
        Debug.Log("Fireball casting mode enabled! Right-click to choose a target.");
    }

    private void Update()
    {
        if (!isCasting)
            return;

        // Detect right-click
        bool rightClicked = Mouse.current != null
            ? Mouse.current.rightButton.wasPressedThisFrame
            : Input.GetMouseButtonDown(1);

        if (rightClicked && !EventSystem.current.IsPointerOverGameObject())
        {
            Vector3? targetPoint = GetMouseGroundPoint();
            if (targetPoint.HasValue)
            {
                SpawnFireball(targetPoint.Value);
                isCasting = false; // exit casting mode
                Debug.Log("Fireball casted!");
            }
            else
            {
                Debug.Log("Right-click missed the ground.");
            }
        }
    }

    private Vector3? GetMouseGroundPoint()
    {
        Vector2 mousePosition = Mouse.current != null
            ? Mouse.current.position.ReadValue()
            : (Vector2)Input.mousePosition;

        Ray ray = mainCamera.ScreenPointToRay(mousePosition);

        RaycastHit hit;

        // If groundLayer is not set, raycast everything
        bool hitSomething = groundLayer.value == 0
            ? Physics.Raycast(ray, out hit, 1000f)
            : Physics.Raycast(ray, out hit, 1000f, groundLayer);

        // Draw ray in Scene view for debugging
        Debug.DrawRay(ray.origin, ray.direction * 1000f, Color.red, 2f);

        if (hitSomething)
        {
            Debug.Log("Raycast hit: " + hit.collider.name);
            return hit.point;
        }

        return null;
    }

    private void SpawnFireball(Vector3 target)
    {
        if (fireballPrefab == null)
        {
            Debug.LogWarning("Fireball prefab not assigned!");
            return;
        }

        // Spawn at caster position
        GameObject fireball = Instantiate(fireballPrefab, transform.position, Quaternion.identity);
        Debug.Log("Fireball spawned at: " + transform.position);

        Vector3 direction = (target - transform.position).normalized;

        if (fireball.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.useGravity = false;   // optional
            rb.linearVelocity = direction * fireballSpeed;
            Debug.Log("Fireball velocity applied: " + rb.linearVelocity);
        }
        else
        {
            Debug.LogWarning("Fireball prefab has no Rigidbody!");
        }
    }
}
