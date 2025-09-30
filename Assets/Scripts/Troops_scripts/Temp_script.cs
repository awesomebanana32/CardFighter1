using UnityEngine;

public class Temp_script : MonoBehaviour
{
    [SerializeField] private GameObject replacementObject; // Object to replace this one with

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            ReplaceSelf();
        }
    }

    private void ReplaceSelf()
    {
        if (replacementObject != null)
        {
            // Instantiate the replacement at the same position and rotation
            Instantiate(replacementObject, transform.position, transform.rotation);

            // Destroy this object
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("Replacement object not assigned!");
        }
    }
}
