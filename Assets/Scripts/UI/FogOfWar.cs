/*
* FogOfWar.cs 
* arnav
* Covers the map in a r * r grid. Determines which grid to show based on the stealth attribute of the character. 
* refers to this tutorial with most of the code from this: https://www.youtube.com/watch?v=TjFTgEpPnQg
*/
using UnityEngine;
using System.Collections.Generic;
public class FogOfWar : MonoBehaviour
{
    public int resolution = 32;
    public int worldSize = 100;
    public float fogHeight = 30f;
    public Color defaultColor = Color.black;
    public Material fogMaterial;

    private FogOfWarTile[,] tiles;

    private Texture2D fogTexture;
    private Color32[] pixels;
    private byte[,] fogData;
    private MeshRenderer fogRenderer;
    private List<VisionSource> sources = new List<VisionSource>();

    void Start()
    {
        //tiles = new FogOfWarTile[resolution,resolution];
        fogTexture = new Texture2D(resolution, resolution, TextureFormat.RGBA32, false);
        fogTexture.filterMode = FilterMode.Bilinear;
        fogData = new byte[resolution, resolution];
        for(int i = 0; i < resolution; i++)
            for(int j = 0; j < resolution; j++)
                fogData[i,j] = 0;
        pixels = new Color32[resolution * resolution];

        for (int i = 0; i < pixels.Length; i++)
            pixels[i] = defaultColor;
        
        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();
        //creates new fogPlace Object
        GameObject fogPlane = GameObject.CreatePrimitive(PrimitiveType.Quad);
        fogPlane.name = "FogPlane";
        fogPlane.transform.position = new Vector3(worldSize / 2f, fogHeight, worldSize / 2f);
        fogPlane.transform.localScale = new Vector3(worldSize, worldSize, 1f);
        fogPlane.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        //adds the fogMaterial and fogTexture to the Object
        fogRenderer = fogPlane.GetComponent<MeshRenderer>();
        fogRenderer.material = fogMaterial;
        fogRenderer.material.mainTexture = fogTexture;
        //Not sure what yet this does
        Vector2 texelSize = new Vector2(1f / resolution, 1f / resolution);
        fogMaterial.SetVector("_TexelSize", new Vector4(texelSize.x, texelSize.y, 0, 0));

        // Apply the fog update immediately
        UpdateFog();
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Debug.Log("Number of sources " + sources.Count);
        UpdateFog();        
    }

    void UpdateFog()
    {
        foreach(VisionSource v in sources)
        {
            RevealCircle(WorldToTex(v.transform.position),v.stealthRadius);
        }
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int index = j * resolution + i;
                byte state = fogData[i, j];

                if (state == 0)
                    pixels[index] = defaultColor;
                else if (state == 1)
                    //pixels[index] = Color.gray; // Optional make grey pixels
                    pixels[index] = Color.clear;
                else
                    pixels[index] = Color.clear;
            }
        }

        fogTexture.SetPixels32(pixels);
        fogTexture.Apply();

        Vector2 texelSize = new Vector2(1f / resolution, 1f / resolution);
        fogMaterial.SetVector("_TexelSize", new Vector4(texelSize.x, texelSize.y, 0, 0));

    }
    void RevealCircle(Vector2Int center, float radiusWorld)
    {
        // Convert the radius from world units to texture pixels
        int radiusTex = Mathf.RoundToInt((radiusWorld / worldSize) * resolution);

        // Get the x and y coordinate of the circle center in texture space
        int cx = center.x;
        int cy = center.y;

        // Loop over a square region that bounds the circle (from -radiusTex to +radiusTex)
        for (int y = -radiusTex; y <= radiusTex; y++)
        {
            for (int x = -radiusTex; x <= radiusTex; x++)
            {
                // Calculate the actual pixel coordinates in the texture
                int fx = cx + x;
                int fy = cy + y;

                // Skip if the pixel is outside the texture boundaries
                if (fx < 0 || fx >= resolution || fy < 0 || fy >= resolution) continue;
  
                if (x * x + y * y <= radiusTex * radiusTex)
                {
                    // Mark this pixel as currently visible (2) in the fogData array
                    fogData[fx, fy] = 2; // Visible
                }
            }
        }
    }
    //Make FogOfWar a singleton
    public void RegisterVisionSource(VisionSource v)
    {
        if(!sources.Contains(v))
            sources.Add(v);
    }

    public void UnRegisterVisionSource(VisionSource v)
    {
        sources.Remove(v);
    }

    void OnDrawGizmosSelected()
    {
        if(fogRenderer == null)
            return;
        Gizmos.color = Color.green;
        Vector3 size = new Vector3(worldSize, 0.1f, worldSize);
        Vector3 fog = fogRenderer.transform.position;
        Gizmos.DrawWireCube(fog, size);
    }

    Vector2Int WorldToTex(Vector3 worldPos)
    {
        float normX = Mathf.InverseLerp(0, worldSize, worldPos.x);
        float normZ = Mathf.InverseLerp(0, worldSize, worldPos.z);
        int x = Mathf.FloorToInt(normX * resolution);
        int y = Mathf.FloorToInt(normZ * resolution);
        return new Vector2Int(x,y);
    }
}
public class VisionSource
{
    public float stealthRadius;
    public Transform transform;
    public VisionSource(Transform t, float s)
    {
        stealthRadius = s;
        transform = t;
    }
}

public class FogOfWarTile : MonoBehaviour
{
    private bool shown = false;
    private GameObject parent;

    void Update()
    {
        if (!shown)
        {
           Hide();
        }
        else
        {
            UnHide();
        }
    }

    void Hide()
    {
        //Do operation to make parent gameObject cover the map tile
    }
    void UnHide()
    {
        //Do operation to show the map, usually just disable
    }
}