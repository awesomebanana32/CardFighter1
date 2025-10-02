using UnityEngine;
   using TMPro; // Use TextMeshPro for better text rendering (or use UnityEngine.UI for standard Text)

   public class PopulationUI : MonoBehaviour
   {
       [SerializeField] private PlacementSystem placementSystem;
       [SerializeField] private TextMeshProUGUI populationText; // Reference to the UI text element

       private void Start()
       {
           if (placementSystem == null)
           {
               Debug.LogError("PlacementSystem is not assigned in PopulationUI.");
               return;
           }

           if (populationText == null)
           {
               Debug.LogError("PopulationText is not assigned in PopulationUI.");
               return;
           }

           UpdatePopulationText();
       }

       private void Update()
       {
           UpdatePopulationText();
       }

       private void UpdatePopulationText()
       {
           populationText.text = $"Population: {placementSystem.CurrentPopulation}/{placementSystem.MaxPopulation}";
       }
   }