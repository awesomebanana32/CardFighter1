using UnityEngine;

public class CapitalCity : MonoBehaviour
{
    public CapitalCityManager manager; // Assign in Inspector

    void OnDestroy()
    {
        if (manager != null)
        {
            manager.CapitalDestroyed(gameObject);
        }
    }
}
