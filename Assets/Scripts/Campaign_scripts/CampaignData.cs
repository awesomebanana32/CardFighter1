using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class CampaignData
{
    public int progress;
    public int wealth;
    public long deck; //every bit determines whether we have a certain card or not there are 64 cards in the database
    public int levelReached;
    public string playerName;
    public CampaignData(string name)
    {
        progress = 0;
        wealth = 0;
        deck = 0;
        levelReached = 3;
        name = playerName;
    }
    public string toString(){
        return JsonUtility.ToJson(this);
    }

}
