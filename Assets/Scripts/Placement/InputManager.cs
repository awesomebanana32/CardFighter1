using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour
{
    [SerializeField] private Camera sceneCamera;
    [SerializeField] private LayerMask placementLayermask;
    [SerializeField] private float fallbackHeight = 0f;

    private Vector3 lastPosition;

    public event Action OnClicked;
    public event Action OnExit;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            OnClicked?.Invoke();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            OnExit?.Invoke();
    }

    public bool IsPointerOverUI()
    {
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }

    public bool GetSelectedMapPosition(out Vector3 position)
    {
        position = lastPosition;

        Vector2 mousePos = Mouse.current != null ? Mouse.current.position.ReadValue() : (Vector2)Input.mousePosition;

        if (sceneCamera == null)
        {
            Debug.LogError("sceneCamera is not assigned!");
            return false;
        }

        Ray ray = sceneCamera.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000, placementLayermask))
        {
            lastPosition = hit.point;
            position = hit.point;
            return true;
        }
        else
        {
            Plane plane = new Plane(Vector3.up, new Vector3(0, fallbackHeight, 0));
            if (plane.Raycast(ray, out float distance))
            {
                position = ray.GetPoint(distance);
                lastPosition = position;
                return true;
            }

            return false;
        }
    }
}
