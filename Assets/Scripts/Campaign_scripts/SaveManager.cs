using UnityEngine;
using System.IO;
//why are we using static here?
public static class SaveManager
{
    //so basically Application.persistentDatapath always ensures a safe file path?
    private static readonly string SAVE_FILE = "/savedata.json";
    private static readonly string SAVE_PATH = Application.persistentDataPath + SAVE_FILE;

    public static bool hasSave()
    {
        return File.Exists(SAVE_PATH);
    }

    public static void SaveGame(CampaignData data)
    {
        try
        {
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SAVE_PATH, json);
            Debug.Log("Saved to:" + SAVE_PATH);

        }
        catch (System.Exception e)
        {
            Debug.Log("Failed Saving: " + e.Message);
        }
    }
    public static CampaignData LoadGame()
    {
        if (File.Exists(SAVE_PATH))
        {
            try
            {
                string json = File.ReadAllText(SAVE_PATH);
                CampaignData save_data = JsonUtility.FromJson<CampaignData>(json);
                return save_data;
            }
            catch (System.Exception e)
            {
                Debug.Log("Failed Reading Save File" + e.Message);
                Debug.Log("Creating New Save File");
                return new CampaignData("BLANK");
            }
        }
        else
        {
            Debug.Log("Creating New Save File");
            return new CampaignData("BLANK");
        }

    }

    public static void DeleteSaveFile()
    {
        if (File.Exists(SAVE_PATH))
        {
            File.Delete(SAVE_PATH);
        }
        else
        {
            Debug.Log("No File Found");
        }
    }

}

