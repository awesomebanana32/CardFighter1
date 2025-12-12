using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class DeckMenu : MonoBehaviour
{
    private long deck;
    public GameObject campaignMenu;
    void OnValidate()
    {
    }
    void Start()
    {
        CampaignData data = SaveManager.LoadGame();
        deck = data.deck;

    }

    void Update()
    {

    }
    public void OpenDeckMenu()
    {
        Debug.Log("Open Deck Menu");
        campaignMenu.SetActive(false);
        gameObject.SetActive(true);
        Camera mainCamera = Camera.main;
        CampaignCameraControl cameraControl = mainCamera.GetComponent<CampaignCameraControl>();
        cameraControl.enable = false;
    }
    public void CloseDeckMenu()
    {
        Debug.Log("Close the Deck Menu");
        campaignMenu.SetActive(true);
        gameObject.SetActive(false);
        Camera mainCamera = Camera.main;
        CampaignCameraControl cameraControl = mainCamera.GetComponent<CampaignCameraControl>();
        cameraControl.enable = true;
    }
}
