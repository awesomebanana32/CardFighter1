using UnityEngine;

public class CircleSelection : MonoBehaviour
{
     [SerializeField] private GameObject selectionCircle;

    public void Select()
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(true);
    }

    public void Deselect()
    {
        if (selectionCircle != null)
            selectionCircle.SetActive(false);
    }
}