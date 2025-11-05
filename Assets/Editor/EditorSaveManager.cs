using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System.IO;
public class EditorSaveManager : EditorWindow
{
    private string UxmlPath = "SavesForm.uxml";

    [MenuItem("Saves/Save Manager")]
    public static void ShowWindow()
    {
        GetWindow<EditorSaveManager>("Save Manager");
    }

    public void CreateGUI()
    {
        string scriptPath = AssetDatabase.GetAssetPath(MonoScript.FromScriptableObject(this));
        string folderPath = Path.GetDirectoryName(scriptPath);
        string path = Path.Combine(folderPath, UxmlPath).Replace('\\', '/');
        Debug.Log(path);
        VisualTreeAsset visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
        if (visualTree == null)
        {
            Debug.LogError("Failed to load UXML file at path: " + path);
            return;
        }
        visualTree.CloneTree(rootVisualElement);

        // --- 3. Get References to Elements ---
        LongField deck = rootVisualElement.Q<LongField>("Deck");
        IntegerField progress = rootVisualElement.Q<IntegerField>("Progress");
        IntegerField wealth = rootVisualElement.Q<IntegerField>("Wealth");
        IntegerField levelField = rootVisualElement.Q<IntegerField>("LevelReached");
        TextField nameField = rootVisualElement.Q<TextField>("Name");

        Button submitButton = rootVisualElement.Q<Button>("SubmitButton");
        if (submitButton != null)
        {
            submitButton.clicked += () =>
            {
                CampaignData data = new CampaignData(nameField.value, progress.value, wealth.value, levelField.value, deck.value);
                SaveManager.SaveGame(data);
            };
        }
        Button clearSave = rootVisualElement.Q<Button>("ClearSave");
        if (clearSave != null)
        {
            clearSave.clicked += () => SaveManager.DeleteSaveFile();
        }

    }
}

