using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TextCore;

public class MainMenuController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGame()
    {
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
