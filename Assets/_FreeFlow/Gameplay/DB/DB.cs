using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DB 
{
    private const string LEVELNUMBERKEY= "LevelNumber";
    
    public static int LevelNumber
    {
        get => PlayerPrefs.GetInt(LEVELNUMBERKEY,1);
        set
        {
            PlayerPrefs.SetInt(LEVELNUMBERKEY, value);
            EventManager.DoFireOnLevelNumberUpdate();

        }
    }
}
