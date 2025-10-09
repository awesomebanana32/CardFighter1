using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class CampaignLevelOpener : MonoBehaviour
{
    public string nextScene;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Scene scene = SceneManager.GetSceneByName(nextScene);
        if (!scene.IsValid())
        {
            //throw Error
            Debug.Log("Scene is Valid");
        }
    }

    // Update is called once per frame
    public void playScene()
    {
        SceneManager.LoadScene(nextScene);
    }
}
