using UnityEngine;

public class DeleteObject : MonoBehaviour
{
    [SerializeField] private GameObject targetObject;

    // Call this from your UI button
    public void DeleteTarget()
    {
        if (targetObject != null)
        {
            Destroy(targetObject); // Deletes the object from the scene
            Debug.Log($"{targetObject.name} has been deleted.");
        }
        else
        {
            Debug.LogWarning("No target object assigned in the inspector!");
        }
    }
}
