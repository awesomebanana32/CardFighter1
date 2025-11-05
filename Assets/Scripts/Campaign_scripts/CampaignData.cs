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
        levelReached = 0;
        playerName = name;
    }
    public CampaignData(string name, int p, int w, int l, long d)
    {
        progress = p;
        wealth = w;
        deck = d;
        levelReached = l;
        playerName = name;
    }

    public string toString(){
        return JsonUtility.ToJson(this);
    }

}
