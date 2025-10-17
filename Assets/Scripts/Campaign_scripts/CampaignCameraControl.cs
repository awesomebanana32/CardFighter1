using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Collections;


public class CampaignCameraControl : MonoBehaviour
{
    public bool enable;
    public Terrain mainMap;
    public float height = 10f;
    private Vector3 cameraFocus;
    private bool wasPressed = false;
    private Vector2 offset;
    public float angle = .57f;
    public void Start()
    {
        Vector3 terrainSize = mainMap.terrainData.size;
        terrainSize /= 2;
        Vector3 terrainCenter = new Vector3(mainMap.transform.position.x + terrainSize.x, mainMap.transform.position.y, mainMap.transform.position.z + terrainSize.z);
        transform.position = terrainCenter + Vector3.up * height;
        transform.LookAt(terrainCenter + Vector3.forward * angle * height);
        cameraFocus = terrainCenter;
        enable = true;
    }

    public void Update()
    {
        if (!enable)
        {
            return;
        }
        if (wasPressed)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            cameraFocus += new Vector3(mousePosition.x-offset.x,0,mousePosition.y-offset.y);
        }
        transform.position = cameraFocus + Vector3.up * height;
        transform.LookAt(cameraFocus + Vector3.forward * angle * height);
        if (Mouse.current.leftButton.isPressed)
        {
            offset = Mouse.current.position.ReadValue();
            wasPressed = true;
        }
        else
        {
            wasPressed = false;
        }
    }





}