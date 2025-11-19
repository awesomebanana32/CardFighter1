using UnityEngine;
using UnityEngine.EventSystems;
public class CitySelectionManager : MonoBehaviour
{
    Camera cam;
    City selectedCity;
    public static City SelectedCity { get; private set; }
    void Start()
    {
        cam = Camera.main;
    }

    void Update()
{
    if (Input.GetMouseButtonDown(0))
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // ignore click over UI
        HandleLeftClick();
    }

    if (Input.GetMouseButtonDown(1))
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return; // ignore right click over UI
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
        if (selectedCity != null)
            selectedCity.ShowRange(false);

        selectedCity = city;
        SelectedCity = city;
        selectedCity.ShowRange(true);

        Debug.Log("Selected city: " + city.name);
    }

    void Deselect()
    {
        if (selectedCity != null)
            selectedCity.ShowRange(false);

        selectedCity = null;
        SelectedCity = null; // <--- NEW
    }
}
