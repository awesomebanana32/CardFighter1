using UnityEngine;

public enum Team
{
    Green,
    Red,
    Gray
}

public static class TeamHelper
{
    public static string[] GetEnemies(Team team)
    {
        switch (team)
        {
            case Team.Green: return new string[] { "TeamRed", "TeamGray" };
            case Team.Red: return new string[] { "TeamGreen", "TeamGray" };
            case Team.Gray: return new string[] { "TeamGreen", "TeamRed" };
            default: return new string[0];
        }
    }
}
