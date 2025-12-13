using UnityEngine;

public class FogOfWarSource : MonoBehaviour
{
    private VisionSource vs;
    void Start()
    {
        vs = new VisionSource(this.transform,5f);
        //TODO: Look up the steath radius from ScriptableObject
        //TODO: Turn the Fog into a singleton for the scene
        GameObject fog = GameObject.Find("Fog");
        if(fog != null)
            fog.GetComponent<FogOfWar>().RegisterVisionSource(vs);
    }

    void OnDestroy()
    {
        Debug.Log("A Fog Source had been destroyed");
        GameObject fog = GameObject.Find("Fog");
        if(fog != null)
            fog.GetComponent<FogOfWar>().UnRegisterVisionSource(vs);
        Start();
    }
}
