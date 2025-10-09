using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // if it is the first time log in to first play scene
        if (SaveManager.hasSave())
        {
            //load scene with save data
            CampaignData data = SaveManager.LoadGame();
            SceneManager.LoadScene("Campaign");
        }
        else
        {
            //load scene for time playing
            //SaveManager.SaveGame(data);
        }
        Debug.Log("GameStarted");

    }

    public void QuitGame()
    {
        Debug.Log("Quit");
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    public void OpenSettings()
    {
        Debug.Log("Settings");
    }

}
