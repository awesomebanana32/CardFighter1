using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.UI;

public class TabSystem : MonoBehaviour
{
    public List<GameObject> tabs;
    public List<String> tabNames;
    private List<Button> buttons;
    private List<int> tabIds;
    public Button prefab;
    void Awake()
    {
        buttons = new List<Button>();
        tabIds = new List<int>();
        for (int i = 0; i < tabs.Count; i++)
        {
            GameObject tab = tabs[i];
            Button newButton = Instantiate<Button>(prefab, this.transform); 
            TextMeshProUGUI buttonText = newButton.GetComponentInChildren<TextMeshProUGUI>(); 
            Debug.Log(tab.name);
            tabIds.Add(i);
            if (buttonText != null)
            {
                buttonText.text = tabNames[i];
            }
            buttons.Add(newButton);
        }
    }

    void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            Button newButton = buttons[i];
            newButton.onClick.AddListener(() => show(newButton));
            tabs[i].SetActive(i == 0);
        }
    }
    public void show(Button button)
    {   
        for(int i = 0; i < tabs.Count; i++)
        {
            tabs[i].SetActive(button == buttons[i]);
        }
    }
    public static void PrintSomething()
    {
        Debug.Log("Printed Something");
    }
}