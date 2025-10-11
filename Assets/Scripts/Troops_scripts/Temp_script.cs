using UnityEngine;
using UnityEngine.UI;

public class Temp_script : MonoBehaviour
{
    [SerializeField] private GameObject replacementObject;
    [SerializeField] private string buttonName = "StartButton"; // Name of your button in the Hierarchy
    private Button startButton;

    void Start()
    {
        // Try to find the button in the scene
        startButton = GameObject.Find(buttonName)?.GetComponent<Button>();

        if (startButton != null)
        {
            startButton.onClick.AddListener(ReplaceSelf);
        }
    }

    private void ReplaceSelf()
    {
        if (replacementObject != null)
        {
            Instantiate(replacementObject, transform.position, transform.rotation);
            Destroy(gameObject);
        }
    }
}
