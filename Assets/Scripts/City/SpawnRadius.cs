using UnityEngine;

public class SpawnRadius : MonoBehaviour
{
    [Header("Building Settings")]
    [SerializeField] private float spawnRadius = 15f; // radius in which buildings can be placed
    [SerializeField] private Transform spawnPoint;

    public float SpawnRadiusValue => spawnRadius;

    public Transform GetSpawnPoint()
    {
        return spawnPoint != null ? spawnPoint : transform;
    }

    public bool IsPointInsideRadius(Vector3 point)
    {
        float dist = Vector3.Distance(transform.position, point);
        return dist <= spawnRadius;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
#endif
}
