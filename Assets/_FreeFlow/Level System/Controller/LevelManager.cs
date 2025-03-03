using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    
    [SerializeField] private GridController _gridController;

    private void Start()
    {
        var level = LevelDataSO.Default.GetLevel(0);
        if (level != null)
        {
            Debug.Log(level);
           
            _gridController.Init(level.GridSize, level);
        }
        else
        {
            Debug.Log(level);
        }

        
    }
}
