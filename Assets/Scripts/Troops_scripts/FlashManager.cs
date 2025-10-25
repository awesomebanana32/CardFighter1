using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FlashManager : MonoBehaviour
{
    public static FlashManager Instance { get; private set; }
    public Color flashColor = Color.red;
    public float flashDuration = 0.2f;

    private Dictionary<GameObject, Coroutine> activeFlashes = new Dictionary<GameObject, Coroutine>();
    private Dictionary<GameObject, List<Color>> originalColorsMap = new Dictionary<GameObject, List<Color>>();

    private void Awake()
    {
        // Singleton setup
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // Ensure FlashManager persists across scenes
        if (transform.parent == null)
        {
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.LogWarning("FlashManager should be a root GameObject to persist across scenes. Drag it to the top level in the hierarchy.");
        }
    }

    public void FlashObject(GameObject obj)
    {
        if (obj == null || !obj.activeInHierarchy) return;

        // Stop any existing flash and restore colors
        if (activeFlashes.TryGetValue(obj, out Coroutine existing))
        {
            StopCoroutine(existing);
            RestoreColors(obj);
            activeFlashes.Remove(obj);
            originalColorsMap.Remove(obj);
        }

        Coroutine flash = StartCoroutine(FlashRoutine(obj));
        activeFlashes[obj] = flash;
    }

    private IEnumerator FlashRoutine(GameObject obj)
    {
        if (obj == null) yield break;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        if (renderers.Length == 0) yield break;

        List<Material> matsToFlash = new List<Material>();
        List<Color> originalColors = new List<Color>();

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            Material[] mats = r.materials; // creates unique instances
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && mats[i].HasProperty("_Color"))
                {
                    matsToFlash.Add(mats[i]);
                    originalColors.Add(mats[i].color);
                    mats[i].color = flashColor;
                }
            }
        }

        originalColorsMap[obj] = originalColors;

        yield return new WaitForSeconds(flashDuration);

        if (obj != null)
            RestoreColors(obj);

        activeFlashes.Remove(obj);
        originalColorsMap.Remove(obj);
    }

    private void RestoreColors(GameObject obj)
    {
        if (obj == null) return;
        if (!originalColorsMap.TryGetValue(obj, out List<Color> originalColors)) return;

        Renderer[] renderers = obj.GetComponentsInChildren<Renderer>(true);
        int colorIndex = 0;

        foreach (Renderer r in renderers)
        {
            if (r == null) continue;

            foreach (Material mat in r.materials)
            {
                if (mat != null && mat.HasProperty("_Color") && colorIndex < originalColors.Count)
                {
                    mat.color = originalColors[colorIndex];
                    colorIndex++;
                }
            }
        }
    }
}
