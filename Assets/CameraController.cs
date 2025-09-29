using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControlActions cameraActions;
    private InputAction movement;
    private Camera camera;
    private Transform cameraTransform;

    // Horizontal motion
    [SerializeField]
    private float maxSpeed = 5f;
    private float speed;
    [SerializeField]
    private float acceleration = 10f;
    [SerializeField]
    private float damping = 15f;

    // FOV zoom
    [SerializeField]
    private float zoomStepSize = 2f;
    [SerializeField]
    private float zoomDampening = 7.5f;
    [SerializeField]
    private float minFOV = 20f;
    [SerializeField]
    private float maxFOV = 60f;
    [SerializeField]
    private float zoomSpeed = 2f;

    // Rotation
    [SerializeField]
    private float maxRotationSpeed = 1f;

    // Drag motion
    [SerializeField]
    private float dragSpeed = 0.01f; // New variable for drag sensitivity
    [SerializeField]
    private float dragDampening = 10f; // New variable for drag smoothing

    private Vector3 targetPosition;
    private float targetFOV;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;
    private Vector3 startDrag;
    private Vector3 dragVelocity; // For smoothing drag movement

    private void Awake()
    {
        cameraActions = new CameraControlActions();
        camera = this.GetComponentInChildren<Camera>();
        cameraTransform = camera.transform;
    }

    private void OnEnable()
    {
        targetFOV = camera.fieldOfView;
        cameraTransform.LookAt(this.transform);
        lastPosition = this.transform.position;
        movement = cameraActions.Camera.Movement;
        cameraActions.Camera.RotateCamera.performed += RotateCamera;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraActions.Camera.Enable();
    }

    private void OnDisable()
    {
        cameraActions.Camera.RotateCamera.performed -= RotateCamera;
        cameraActions.Camera.ZoomCamera.performed -= ZoomCamera;
        cameraActions.Disable();
    }

    private void Update()
    {
        GetKeyboardMovement();
        DragCamera();
        UpdateVelocity();
        UpdateCameraPosition();
        UpdateBasePosition();
    }

    private void UpdateVelocity()
    {
        horizontalVelocity = (this.transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0;
        lastPosition = this.transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = movement.ReadValue<Vector2>().x * GetCameraRight() + movement.ReadValue<Vector2>().y * GetCameraForward();
        inputValue = inputValue.normalized;
        if (inputValue.sqrMagnitude > 0.01f)
        {
            targetPosition += inputValue;
        }
    }

    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0;
        return right;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0;
        return forward;
    }

    private void UpdateBasePosition()
    {
        if (targetPosition.sqrMagnitude > 0.1f)
        {
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * acceleration);
            transform.position += targetPosition * speed * Time.deltaTime;
        }
        else
        {
            horizontalVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, Time.deltaTime * damping);
            transform.position += horizontalVelocity * Time.deltaTime;
        }
        targetPosition = Vector3.zero;
    }

    private void RotateCamera(InputAction.CallbackContext inputValue)
    {
        if (!Mouse.current.middleButton.isPressed)
        {
            return;
        }
        float value = inputValue.ReadValue<Vector2>().x;
        transform.rotation = Quaternion.Euler(0f, value * maxRotationSpeed + transform.rotation.eulerAngles.y, 0f);
    }

    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y * zoomStepSize;
        if (Mathf.Abs(value) > 0.01f)
        {
            Debug.Log($"Zoom input: {value}, targetFOV: {targetFOV}");
            targetFOV = camera.fieldOfView + value;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }
    }

    private void UpdateCameraPosition()
    {
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime * zoomDampening);
        cameraTransform.LookAt(this.transform);
    }

    private void DragCamera()
    {
        if (!Mouse.current.rightButton.isPressed)
        {
            dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, Time.deltaTime * dragDampening);
            return;
        }

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                startDrag = ray.GetPoint(distance);
            }
            else
            {
                Vector3 dragDelta = (startDrag - ray.GetPoint(distance)) * dragSpeed;
                dragVelocity = Vector3.Lerp(dragVelocity, dragDelta, Time.deltaTime * dragDampening);
                targetPosition += dragVelocity;
            }
        }
    }
}