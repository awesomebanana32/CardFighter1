using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    private CameraControlActions cameraActions;
    private InputAction movement;
    private Camera mainCamera;
    private Transform cameraTransform;

    [Header("Initial Camera Settings")]
    [SerializeField] private Vector3 startingRotation = new Vector3(60f, 0f, 0f);
    [SerializeField] private Vector3 startingPositionOffset = new Vector3(0f, 0f, 0f);

    // Horizontal motion
    [SerializeField] private float maxSpeed = 5f;
    private float speed;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float damping = 15f;

    // FOV zoom
    [SerializeField] private float zoomStepSize = 2f;
    [SerializeField] private float zoomDampening = 7.5f;
    [SerializeField] private float minFOV = 20f;
    [SerializeField] private float maxFOV = 60f;

    // Edge scrolling
    [SerializeField] private float edgeTolerance = 0.05f;

    // Drag motion (COMMENTED OUT)
    /*
    [SerializeField] private float dragSpeed = 0.01f;
    [SerializeField] private float dragDampening = 10f;
    */

    private Vector3 targetPosition;
    private float targetFOV;
    private Vector3 horizontalVelocity;
    private Vector3 lastPosition;

    // Drag variables (COMMENTED OUT)
    /*
    private Vector3 startDrag;
    private Vector3 dragVelocity;
    */

    private void Awake()
    {
        cameraActions = new CameraControlActions();
        mainCamera = GetComponentInChildren<Camera>();
        cameraTransform = mainCamera.transform;

        // Apply custom start transform
        cameraTransform.localRotation = Quaternion.Euler(startingRotation);
        cameraTransform.localPosition += startingPositionOffset;
    }

    private void OnEnable()
    {
        targetFOV = mainCamera.fieldOfView;
        lastPosition = transform.position;
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
        if (Time.timeScale == 0f) return;

        GetKeyboardMovement();
        // DragCamera(); // Commented out
        UpdateVelocity();
        UpdateCameraPosition();
        // CheckMouseAtScreenEdge(); // Commented out
        UpdateBasePosition();
    }

    private void UpdateVelocity()
    {
        if (Time.deltaTime <= 0f) return;

        horizontalVelocity = (transform.position - lastPosition) / Time.deltaTime;
        horizontalVelocity.y = 0f;
        lastPosition = transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector2 inputVec = movement.ReadValue<Vector2>();
        Vector3 inputValue = inputVec.x * GetCameraRight() + inputVec.y * GetCameraForward();
        inputValue = inputValue.normalized;

        if (inputValue.sqrMagnitude > 0.01f)
            targetPosition += inputValue;
    }

    private Vector3 GetCameraRight()
    {
        Vector3 right = cameraTransform.right;
        right.y = 0f;
        return right.normalized;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forward = cameraTransform.forward;
        forward.y = 0f;
        return forward.normalized;
    }

    private void UpdateBasePosition()
    {
        if (Time.deltaTime <= 0f) return;

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
            targetFOV = Mathf.Clamp(mainCamera.fieldOfView + value, minFOV, maxFOV);
        }
    }

    private void UpdateCameraPosition()
    {
        if (Time.deltaTime > 0f)
        {
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, targetFOV, Time.deltaTime * zoomDampening);
        }
    }

    /*
    private void CheckMouseAtScreenEdge()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 viewportPos = mainCamera.ScreenToViewportPoint(mousePosition);
        Vector3 moveDirection = Vector3.zero;

        if (viewportPos.x < edgeTolerance)
            moveDirection -= GetCameraRight();
        else if (viewportPos.x > 1f - edgeTolerance)
            moveDirection += GetCameraRight();

        if (viewportPos.y < edgeTolerance)
            moveDirection -= GetCameraForward();
        else if (viewportPos.y > 1f - edgeTolerance)
            moveDirection += GetCameraForward();

        // Normalize diagonal movement
        if (moveDirection.sqrMagnitude > 0.01f)
            moveDirection.Normalize();

        targetPosition += moveDirection;
    }
    */

    // DragCamera method (COMMENTED OUT)
    /*
    private void DragCamera()
    {
        if (Time.deltaTime <= 0f) return;

        if (!Mouse.current.rightButton.isPressed)
        {
            dragVelocity = Vector3.Lerp(dragVelocity, Vector3.zero, Time.deltaTime * dragDampening);
            return;
        }

        Plane plane = new Plane(Vector3.up, Vector3.zero);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (plane.Raycast(ray, out float distance))
        {
            if (Mouse.current.rightButton.wasPressedThisFrame)
                startDrag = ray.GetPoint(distance);
            else
            {
                Vector3 dragDelta = (startDrag - ray.GetPoint(distance)) * dragSpeed;
                dragVelocity = Vector3.Lerp(dragVelocity, dragDelta, Time.deltaTime * dragDampening);
                targetPosition += dragVelocity;
            }
        }
    }
    */
}
