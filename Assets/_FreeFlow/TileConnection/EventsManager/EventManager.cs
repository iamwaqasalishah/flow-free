using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventManager
{
    public static event Action<GridTile> OnStartPath;
    public static void DoFireOnStartPath(GridTile tile)=> OnStartPath?.Invoke(tile);
   
    public static event Action<GridTile> OnTileAdded;
    public static void DoFireOnTileAdded(GridTile tile)=> OnTileAdded?.Invoke(tile);
   
    public static event Action OnEndPath;
    public static void DoFireOnEndPath() => OnEndPath?.Invoke();
}
