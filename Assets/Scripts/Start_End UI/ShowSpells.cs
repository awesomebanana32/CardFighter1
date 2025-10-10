using UnityEngine;

public class ShowSpells : MonoBehaviour
{
     [SerializeField] private GameObject targetObject; // The object to show

    // This function will be called by the Button's OnClick event
    public void ShowObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(true); // Makes the object visible
        }
        else
        {
            Debug.LogWarning("No target object assigned!");
        }
    }
}
