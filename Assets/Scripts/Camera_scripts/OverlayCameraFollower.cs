using UnityEngine;

[RequireComponent(typeof(Camera))]
public class OverlayCameraFollower : MonoBehaviour
{
    public Camera baseCamera;

    private Camera overlayCam;

    private void Awake()
    {
        overlayCam = GetComponent<Camera>();
    }

    private void LateUpdate()
    {
        if (baseCamera == null) return;
        overlayCam.fieldOfView = baseCamera.fieldOfView;
        overlayCam.transform.position = baseCamera.transform.position;
        overlayCam.transform.rotation = baseCamera.transform.rotation;
    }
}
