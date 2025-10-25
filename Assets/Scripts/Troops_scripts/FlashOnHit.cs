using UnityEngine;

public class FlashOnHit : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private Renderer rend;
    private Color originalColor;
    private Material material;

    private void Awake()
    {
        // Find the first renderer in children (so it works on models nested under the root)
        rend = GetComponentInChildren<Renderer>();
        if (rend != null)
        {
            material = rend.material; // creates instance
            originalColor = material.color;
        }
        else
        {
            Debug.LogWarning($"{name} has no Renderer found for FlashOnHit!");
        }
    }

    public void Flash()
    {
        if (rend == null) return;
        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        material.color = originalColor;
    }
}
