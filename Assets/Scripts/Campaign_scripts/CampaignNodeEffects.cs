using UnityEngine;
using UnityEngine.InputSystem;

public class CampaignNodeEffects : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // 2. Create a Ray
            // A Ray is a line segment starting from the camera and going 
            // through the mouse position into the scene.
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 3. Define the Hit Info
            // This variable will store all the information about the 
            // object that the ray hits.
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
                // Example Action: Change the material color of the clicked object
                Renderer renderer = clickedObject.GetComponent<Renderer>();

                if (clickedObject.transform == transform)
                {
                    OnClick();
                }
                if (renderer != null)
                    {
                        renderer.material.color = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
                    }

                // You can call a function on the clicked object's script here:
                // ExampleScript targetScript = clickedObject.GetComponent<ExampleScript>();
                // if (targetScript != null)
                // {
                //     targetScript.TakeDamage(); 
                // }
            }
        }
    }

    // Update is called once per frame
    void OnClick()
    {
        GetComponent<CampaignLevelOpener>().playScene();
    }
}
