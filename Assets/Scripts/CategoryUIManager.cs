using UnityEngine;

public class CategoryUIManager : MonoBehaviour
{
    public GameObject spellsPanel;
    public GameObject troopsPanel;
    public GameObject buildingsPanel;

    public void ShowSpells()
    {
        spellsPanel.SetActive(true);
        troopsPanel.SetActive(false);
        buildingsPanel.SetActive(false);
    }

    public void ShowTroops()
    {
        spellsPanel.SetActive(false);
        troopsPanel.SetActive(true);
        buildingsPanel.SetActive(false);
    }

    public void ShowBuildings()
    {
        spellsPanel.SetActive(false);
        troopsPanel.SetActive(false);
        buildingsPanel.SetActive(true);
    }
}
