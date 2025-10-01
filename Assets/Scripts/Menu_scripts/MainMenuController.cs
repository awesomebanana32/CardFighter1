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
        }
        else
        {
            //load scene for time playing
        }
        CampaignData data = SaveManager.LoadGame();
        SaveManager.SaveGame(data);
        Debug.Log("GameStarted");
        SceneManager.LoadScene("Battle");

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
