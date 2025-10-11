using UnityEngine;

public class Placement_button_disapear : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; // The object to hide

    // This function will be called by the Button's OnClick event
    public void HideObject()
    {
        if (targetObject != null)
        {
            targetObject.SetActive(false); // Makes the object disappear
        }
    }
}