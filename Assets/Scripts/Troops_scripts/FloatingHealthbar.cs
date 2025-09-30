using UnityEngine;
using UnityEngine.UI;

public class FloatingHealthbar : MonoBehaviour
{
    [SerializeField] private Slider slider;   
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 offset;
    private Camera mainCamera;

    void Awake()
    {
        // Automatically find the main camera
        if (mainCamera == null)
            mainCamera = Camera.main;
    }

    public void UpdateHealthBar(float currentValue, float maxValue)
    {
        slider.value = currentValue / maxValue;
    }
    void Update()
    {
       transform.rotation = mainCamera.transform.rotation;
       transform.position = target.position + offset;
    }
}
