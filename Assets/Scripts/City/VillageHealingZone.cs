using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VillageHealingZone : MonoBehaviour
{
    [Header("Healing Settings")]
    [SerializeField] private float healAmountPerSecond = 5f;
    [SerializeField] private float healingRadius = 5f;
    [SerializeField] private string healingTag = "TeamGreen";
    [SerializeField] private LayerMask healableLayers; // exclude Buildings layer

    [Header("Visual Settings")]
    [SerializeField] private int circleSegments = 50;
    [SerializeField] private Color circleColor = Color.green;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();

        lineRenderer.positionCount = circleSegments + 1;
        lineRenderer.loop = true;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = circleColor;
        lineRenderer.endColor = circleColor;
        lineRenderer.enabled = false;

        DrawCircle();
    }

    private void Update()
    {
        bool anyTroopInRange = false;

        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            healingRadius,
            healableLayers
        );

        foreach (Collider col in hitColliders)
        {
            if (!col.CompareTag(healingTag))
                continue;

            TroopHealth troopHealth = col.GetComponent<TroopHealth>();
            if (troopHealth == null)
                continue;

            troopHealth.Heal(healAmountPerSecond * Time.deltaTime);
            anyTroopInRange = true;
        }

        lineRenderer.enabled = anyTroopInRange;
    }

    private void DrawCircle()
    {
        float angle = 0f;

        for (int i = 0; i <= circleSegments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * healingRadius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * healingRadius;

            lineRenderer.SetPosition(
                i,
                new Vector3(x, 0.05f, z) + transform.position
            );

            angle += 360f / circleSegments;
        }
    }

    private void OnValidate()
    {
        if (lineRenderer != null)
            DrawCircle();
    }
}
