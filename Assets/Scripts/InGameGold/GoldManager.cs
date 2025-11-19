using UnityEngine;
using System;

public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance;

    [Header("Gold Settings")]
    [SerializeField] private int startingGold = 500;
    [SerializeField] private int goldPerSecond = 5;

    public int CurrentGold { get; private set; }

    float timer;

    public event Action<int> OnGoldChanged;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        CurrentGold = startingGold;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= 1f)
        {
            timer -= 1f;
            AddGold(goldPerSecond);
        }
    }

    public void AddGold(int amount)
    {
        CurrentGold += amount;
        OnGoldChanged?.Invoke(CurrentGold);
    }

    public bool SpendGold(int amount)
    {
        if (CurrentGold < amount)
            return false;

        CurrentGold -= amount;
        OnGoldChanged?.Invoke(CurrentGold);
        return true;
    }
}
