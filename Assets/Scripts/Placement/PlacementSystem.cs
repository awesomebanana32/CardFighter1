using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlacementSystem : MonoBehaviour
{
    [SerializeField] private ObjectDatabaseSO database;
    [SerializeField] private int maxPopulation = 100;

    [Header("UI")]
    [SerializeField] private GameObject selectCityPopup; // Assign a UI Text or panel here
    [SerializeField] private float popupDuration = 2f;

    private int currentPopulation = 0;

    public static PlacementSystem Instance { get; private set; }

    public int MaxPopulation => maxPopulation;
    public int CurrentPopulation => currentPopulation;

    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Call this when a troop button is clicked
    public void SpawnTroop(int troopID)
    {
        City selectedCity = CitySelectionManager.SelectedCity;

        if (selectedCity == null)
        {
            ShowSelectCityPopup();
            return;
        }

        GameObject troopPrefab = database.GetPrefabByID(troopID);
        int popCost = database.GetPopulationCostByID(troopID);

        if (troopPrefab == null)
        {
            Debug.LogWarning("Troop prefab not found for ID: " + troopID);
            return;
        }

        if (currentPopulation + popCost > maxPopulation)
        {
            Debug.LogWarning("Not enough population to spawn this troop!");
            return;
        }

        // Spawn at the city's spawn point
        Transform spawnPoint = selectedCity.GetSpawnPoint();
        Instantiate(troopPrefab, spawnPoint.position, Quaternion.identity);

        AddToPopulation(popCost);
    }

    public void AddToPopulation(int amount)
    {
        currentPopulation += amount;
        currentPopulation = Mathf.Clamp(currentPopulation, 0, maxPopulation);
    }

    // Show a brief popup warning if no city is selected
    private void ShowSelectCityPopup()
    {
        if (selectCityPopup == null) return;

        selectCityPopup.SetActive(true);
        StopAllCoroutines(); // ensure multiple clicks don’t overlap timers
        StartCoroutine(HidePopupAfterDelay(popupDuration));
    }

    private IEnumerator HidePopupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (selectCityPopup != null)
            selectCityPopup.SetActive(false);
    }
}
