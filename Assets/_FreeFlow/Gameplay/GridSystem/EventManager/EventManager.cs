using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventManager 
{
  public static event Action<int,LevelData> OnInitializeGrid;
  public static void DoFireOnInitializeGrid(int gridSize,LevelData levelData) => OnInitializeGrid?.Invoke(gridSize,levelData);
  
}
