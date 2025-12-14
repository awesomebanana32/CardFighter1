using UnityEngine;
using System.Collections.Generic;

public class FogOfWarDiscovered : MonoBehaviour
{
    public static FogOfWarDiscovered Instance;

    [Header("Fog Settings")]
    public int resolution = 32;
    public int worldSize = 100;
    public float fogHeight = 29.5f; // slightly above ground but below visible fog

    [Header("Fog Appearance")]
    public Material fogMaterial;

    private Texture2D fogTexture;
    private Color32[] pixels;
    private byte[,] discoveredData;
    private MeshRenderer fogRenderer;
    private List<FogOfWarSource> sources = new List<FogOfWarSource>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        fogTexture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        fogTexture.filterMode = FilterMode.Bilinear;
        fogTexture.wrapMode = TextureWrapMode.Clamp;

        pixels = new Color32[resolution * resolution];
        discoveredData = new byte[resolution, resolution];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = new Color32(0, 0, 0, 255); // start fully black

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();

        GameObject fogPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fogPlane.name = "FogPlaneDiscovered";
        fogPlane.transform.position = new Vector3(worldSize / 2f, fogHeight, worldSize / 2f);
        fogPlane.transform.localScale = new Vector3(worldSize, worldSize, 1f);
        fogPlane.transform.rotation = Quaternion.Euler(90f, 0f, 0f);

        fogRenderer = fogPlane.GetComponent<MeshRenderer>();
        fogRenderer.material = fogMaterial;
        fogRenderer.material.mainTexture = fogTexture;

        UpdateTexelSize();
    }

    void LateUpdate()
    {
        foreach (var source in sources)
        {
            if (source == null) continue;
            RevealCircle(WorldToTex(source.transform.position), source.stealthRadius);
        }

        UpdateFogTexture();
    }

    void UpdateFogTexture()
    {
        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                int index = y * resolution + x;
                pixels[index] = discoveredData[x, y] == 1 ? new Color32(0, 0, 0, 0) : new Color32(0, 0, 0, 255);
            }
        }

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();
        UpdateTexelSize();
    }

    void UpdateTexelSize()
    {
        Vector2 texelSize = new Vector2(1f / resolution, 1f / resolution);
        fogMaterial.SetVector("_TexelSize", new Vector4(texelSize.x, texelSize.y, 0, 0));
    }

    void RevealCircle(Vector2Int center, float radiusWorld)
    {
        int radiusTex = Mathf.RoundToInt((radiusWorld / worldSize) * resolution);

        for (int y = -radiusTex; y <= radiusTex; y++)
        {
            for (int x = -radiusTex; x <= radiusTex; x++)
            {
                int fx = center.x + x;
                int fy = center.y + y;

                if (fx < 0 || fx >= resolution || fy < 0 || fy >= resolution) continue;

                if (x * x + y * y <= radiusTex * radiusTex)
                    discoveredData[fx, fy] = 1;
            }
        }
    }

    Vector2Int WorldToTex(Vector3 worldPos)
    {
        float normX = Mathf.InverseLerp(0, worldSize, worldPos.x);
        float normZ = Mathf.InverseLerp(0, worldSize, worldPos.z);

        int x = Mathf.Clamp(Mathf.FloorToInt(normX * resolution), 0, resolution - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(normZ * resolution), 0, resolution - 1);

        return new Vector2Int(x, y);
    }

    public void RegisterVisionSource(FogOfWarSource source)
    {
        if (!sources.Contains(source)) sources.Add(source);
    }

    public void UnRegisterVisionSource(FogOfWarSource source)
    {
        sources.Remove(source);
    }
}
