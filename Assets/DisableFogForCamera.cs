using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class MinimapNoFog : MonoBehaviour
{
    private Camera cam;

    void OnEnable()
    {
        cam = GetComponent<Camera>();
        // Turn off fog for this camera only
        RenderPipelineManager.beginCameraRendering += DisableFog;
    }

    void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= DisableFog;
    }

    void DisableFog(ScriptableRenderContext context, Camera renderingCamera)
    {
        if (renderingCamera == cam)
        {
            RenderSettings.fog = false;
        }
    }
}
