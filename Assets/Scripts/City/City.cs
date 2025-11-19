using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] Transform troopSpawnPoint;
    public GameObject rangeCircle;

    void Start()
    {
        if (rangeCircle != null)
            rangeCircle.SetActive(false);
    }
    public Transform GetSpawnPoint()
    {
        return troopSpawnPoint != null ? troopSpawnPoint : transform;
    }
    public void ShowRange(bool show)
    {
        if (rangeCircle != null)
            rangeCircle.SetActive(show);
    }

}
