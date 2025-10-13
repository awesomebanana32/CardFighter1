using UnityEngine;
using UnityEngine.InputSystem;

public class CampaignNodeEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        // 2. Create a Ray
        // A Ray is a line segment starting from the camera and going 
        // through the mouse position into the scene.
        // This variable will store all the information about the 
        // object that the ray hits.
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        // 4. Perform the Raycast
        // Physics.Raycast attempts to fire the ray. 
        // - 'ray' is the starting line.
        // - 'out hit' means if it hits something, put the info in the 'hit' variable.
        // - '100f' is the max distance the ray can travel (optional).
        if (Physics.Raycast(ray, out hit, 500f))
        {
            // We successfully hit a 3D object!
            // 5. Access the Hit Object
            GameObject clickedObject = hit.collider.gameObject;
            Renderer renderer = this.GetComponent<Renderer>();
            if (clickedObject.transform ==this.transform)
            {
                if (GetComponent<Renderer>() != null)
                {
                    GetComponent<Renderer>().material.color = Color.gold;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if (clickedObject.transform == transform)
                    {
                        OnClick();
                    }
                    if (GetComponent<Renderer>() != null)
                    {
                        this.GetComponent<Renderer>().material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    }
                }
            }
            else
            {
                this.GetComponent<Renderer>().material.color = Color.gray;
            }

        }
        else
        {
                this.GetComponent<Renderer>().material.color = Color.gray;
        }
    }

    // Update is called once per frame
    void OnClick()
    {
        GetComponent<CampaignLevelOpener>().playScene();
    }
}
