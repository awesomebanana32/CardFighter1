using UnityEngine;

public class FogOfWarSource : MonoBehaviour
{
    private VisionSource vs;
    void Start()
    {
        vs = new VisionSource(this.transform,5f);
        //TODO: Look up the steath radius from ScriptableObject
        GameObject fog = GameObject.Find("Fog");
        if(fog != null)
            fog.GetComponent<FogOfWar>().RegisterVisionSource(vs);
    }

    void OnDeath()
    {

    }
}
