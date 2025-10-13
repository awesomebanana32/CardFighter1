using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TerrainGeneration : MonoBehaviour
{
    [SerializeField]
    public TerrainType[] terrainTypes;
    private MeshRenderer tileRender;
    [SerializeField]
    private MeshFilter tileFilter;
    private MeshCollider tileCollider;
    [SerializeField]
    public float heightMultiplayer;
    [SerializeField]
    private AnimationCurve heightCurve;
    [SerializeField]
    public Wave[] waves;
    public float mapScale;
    private int tileWidth;
    private int tileHeight;
    [ExecuteAlways]
    void OnValidate()
    {
        tileCollider = GetComponent<MeshCollider>();
        tileRender = GetComponent<MeshRenderer>();
        CreateTile();
    }
    void CreateTile()
    {
        TerrainData data = GetComponent<Terrain>().terrainData;
        tileHeight = data.heightmapResolution;
        tileWidth = tileHeight;
        float offsetZ = -transform.position.x;
        float offsetX = -transform.position.z;

        float[,] hmap = NoiseMapGeneration.GenerateNoiseMap(tileHeight, tileWidth, mapScale, offsetX, offsetZ, waves);
        for (int i = 0; i < tileHeight; i++)
        {
            for (int j = 0; j < tileWidth; j++)
            {
                hmap[i, j] = this.heightCurve.Evaluate(hmap[i, j]) * heightMultiplayer;
            }
        }
        data.SetHeights(0, 0, hmap);
        Texture2D finalTexture = BuildTexture(hmap);
        TerrainLayer[] layers = new TerrainLayer[terrainTypes.Length];
        int resolution = data.alphamapResolution;
        int l = 0;
        foreach (TerrainType tType in terrainTypes)
        {
            TerrainLayer tLayer = new TerrainLayer();
            Texture2D tex = new Texture2D(tileHeight, tileWidth);
            Color[] cmap = new Color[tileHeight * tileWidth];
            int c = 0;
            for (int i = 0; i < resolution; i++)
            {
                for (int j = 0; j < resolution; j++)
                {
                    cmap[c] = tType.color;
                    c++;
                }
            }
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.SetPixels(cmap);
            tex.Apply();

            tLayer.diffuseTexture = tex;
            tLayer.tileSize = new Vector2(10f, 10f);
            layers[l] = tLayer;
            l++;
        }
        float[,,] splatMapWeights = new float[resolution, resolution, layers.Length];
        for (int i = 0; i < resolution; i++)
        {
            for (int j = 0; j < resolution; j++)
            {
                int largestIndex = 0;
                for (int k = 0; k < terrainTypes.Length; k++)
                {
                    splatMapWeights[i, j, k] = 0f;
                    if (hmap[i, j] > terrainTypes[k].height)
                    {
                        largestIndex = k;
                    }
                }
                splatMapWeights[i, j, largestIndex] = 1.0f;
            }
        }
        data.terrainLayers = layers;
        data.SetAlphamaps(0, 0, splatMapWeights);
        GetComponent<Terrain>().Flush();
    }
    private Texture2D BuildTexture(float[,] noiseMap)
    {
        Texture2D tex = new Texture2D(tileHeight, tileWidth);
        Color[] cmap = new Color[tileHeight * tileWidth];
        int c = 0;
        for (int i = 0; i < tileHeight; i++)
        {
            for (int j = 0; j < tileWidth; j++)
            {
                TerrainType terrain = ReturnType(noiseMap[i, j]);
                cmap[c] = terrain.color;
                c++;
            }
        }
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.SetPixels(cmap);
        tex.Apply();
        return tex;
    }
    TerrainType ReturnType(float h)
    {
        foreach (TerrainType terrain in terrainTypes)
        {
            if (h < terrain.height)
            {
                return terrain;
            }
        }
        return terrainTypes[terrainTypes.Length - 1];
    }
}
