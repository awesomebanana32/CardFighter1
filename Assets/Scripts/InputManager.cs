using UnityEngine;
using UnityEngine.InputSystem; // For new Input System compatibility

public class InputManager : MonoBehaviour
{
    [SerializeField]
    private Camera sceneCamera;

    private Vector3 lastPosition;

    [SerializeField]
    private LayerMask placementLayermask;

    [SerializeField]
    private float fallbackHeight = 0f; // Adjust to match terrain's base height

    public bool GetSelectedMapPosition(out Vector3 position)
    {
        position = lastPosition;

        // Use new Input System for consistency with CameraController
        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : Input.mousePosition;
        if (sceneCamera == null)
        {
            Debug.LogError("sceneCamera is not assigned!");
            return false;
        }

        Ray ray = sceneCamera.ScreenPointToRay(mousePos);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 1000, placementLayermask))
        {
            lastPosition = hit.point;
            position = hit.point;
            Debug.Log($"Raycast hit: {hit.collider.name} at {hit.point} on layer {LayerMask.LayerToName(hit.collider.gameObject.layer)}");
            return true;
        }
        else
        {
            // Fallback to a plane at terrain's base height
            Plane plane = new Plane(Vector3.up, new Vector3(0, fallbackHeight, 0));
            if (plane.Raycast(ray, out float distance))
            {
                position = ray.GetPoint(distance);
                lastPosition = position;
                Debug.Log($"Plane hit at: {position}");
                return true;
            }
            Debug.LogWarning($"Raycast failed. Mouse pos: {mousePos}, LayerMask: {LayerMask.LayerToName(placementLayermask.value)}");
            Debug.DrawRay(ray.origin, ray.direction * 1000, Color.red, 1f);
            return false;
        }
    }
}