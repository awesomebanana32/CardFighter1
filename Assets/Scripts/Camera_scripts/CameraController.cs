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
        // Set initial camera orientation
        cameraTransform.localRotation = Quaternion.identity;
    }

    private void OnEnable()
    {
        targetFOV = camera.fieldOfView;
        lastPosition = this.transform.position;
        movement = cameraActions.Camera.Movement;
        cameraActions.Camera.ZoomCamera.performed += ZoomCamera;
        cameraActions.Camera.Enable();
    }

    private void OnDisable()
    {
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

    private void ZoomCamera(InputAction.CallbackContext inputValue)
    {
        float value = -inputValue.ReadValue<Vector2>().y * zoomStepSize;
        if (Mathf.Abs(value) > 0.01f)
        {
            targetFOV = camera.fieldOfView + value;
            targetFOV = Mathf.Clamp(targetFOV, minFOV, maxFOV);
        }
    }

    private void UpdateCameraPosition()
    {
        // Update FOV only, let the camera inherit rotation from the parent
        camera.fieldOfView = Mathf.Lerp(camera.fieldOfView, targetFOV, Time.deltaTime * zoomDampening);
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