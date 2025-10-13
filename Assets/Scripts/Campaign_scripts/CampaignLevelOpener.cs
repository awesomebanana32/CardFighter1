using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using System.Data.Common;

public class CampaignLevelOpener : MonoBehaviour
{
    public string nextScene;
    public int currentLevel;
    private CampaignLevelType currentState;
    void Start()
    {
        Scene scene = SceneManager.GetSceneByName(nextScene);
        CampaignData data = SaveManager.LoadGame();
        Debug.Log(data.levelReached);
        if(data.levelReached >= currentLevel)
        {
            currentState = CampaignLevelType.UNLOCKED;
        }
        else
        {
            currentState = CampaignLevelType.LOCKED;
        }
        if (scene.IsValid())
        {
            throw new System.Exception();
        }
    }
    public void playScene()
    {
        SceneManager.LoadScene(nextScene);
    }
    public CampaignLevelType getState()
    {
        return currentState;
    }
}
