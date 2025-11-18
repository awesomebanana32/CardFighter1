using UnityEngine;

public class CitySelectionManager : MonoBehaviour
{
    Camera cam;
    City selectedCity;

    void Start()
    {
        cam = Camera.main;
    }

    void Update()
    {
        // LEFT CLICK
        if (Input.GetMouseButtonDown(0))
        {
            HandleLeftClick();
        }

        // RIGHT CLICK deselect
        if (Input.GetMouseButtonDown(1))
        {
            Deselect();
        }
    }

    void HandleLeftClick()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            City city = hit.collider.GetComponent<City>();

            if (city != null)
            {
                SelectCity(city);
                return;
            }
        }

        // Clicked empty space
        Deselect();
    }

    void SelectCity(City city)
    {
        // Deselect old
        if (selectedCity != null)
            selectedCity.ShowRange(false);

        // Select new
        selectedCity = city;
        selectedCity.ShowRange(true);
    }

    void Deselect()
    {
        if (selectedCity != null)
            selectedCity.ShowRange(false);

        selectedCity = null;
    }
}
