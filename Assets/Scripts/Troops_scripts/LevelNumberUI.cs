using TMPro;
using UnityEngine;

public class LevelNumberUI : MonoBehaviour
{
    public TextMeshProUGUI text;  // Canvas text

    private void Awake()
    {
        if (text == null)
            text = GetComponent<TextMeshProUGUI>();
    }

    public void UpdateLevelNumber(int level)
    {
        if (text != null)
            text.text = level.ToString();
    }
}
