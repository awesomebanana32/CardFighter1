using UnityEngine;
using System;
using System.Runtime.InteropServices.WindowsRuntime;

[System.Serializable]
public class CampaignData
{
    public int _progress;
    public int _wealth;
    public long _deck;
    public int _levelReached;
    public string _name;
    public CampaignData(string name)
    {
        _progress = 0;
        _wealth = 0;
        _deck = 0;
        _levelReached = 0;
        _name = name;
    }
    public string toString(){
        return JsonUtility.ToJson(this);
    }

}
