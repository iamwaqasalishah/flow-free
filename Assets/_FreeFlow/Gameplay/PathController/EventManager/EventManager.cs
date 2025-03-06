using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class EventManager
{
    public static event Action OnUndo;
    public static void DoFireOnUndo() => OnUndo?.Invoke(); 
   
    public static event Action OnReset;
    public static void DoFireOnReset() => OnReset?.Invoke(); 
    
    public static event Action OnValidatePath;
    public static void DoFireOnValidatePath() => OnValidatePath?.Invoke(); 
    
    public static event Action<GridTile> OnStartNewPath;
    public static void DoFireOnStartNewPath(GridTile gridTile) => OnStartNewPath?.Invoke(gridTile); 
    
    public static event Action<GridTile> OnHandleTileSelection;
    public static void DoFireOnHandleTileSelection(GridTile gridTile) => OnHandleTileSelection?.Invoke(gridTile);
    
}
