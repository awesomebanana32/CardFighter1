using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI Instance;

    public GameObject panel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI statsText;

    private void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void Show(string title, string stats, Vector3 pos)
    {
        panel.SetActive(true);
        titleText.text = title;
        statsText.text = stats;
        panel.transform.position = pos;
    }

    public void Hide()
    {
        panel.SetActive(false);
    }
}
