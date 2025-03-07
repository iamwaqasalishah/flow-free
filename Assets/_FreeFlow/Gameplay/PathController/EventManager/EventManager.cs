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
    
    public static event Action<IGridTile> OnStartNewPath;
    public static void DoFireOnStartNewPath(IGridTile gridTile) => OnStartNewPath?.Invoke(gridTile); 
    
    public static event Action<IGridTile> OnHandleTileSelection;
    public static void DoFireOnHandleTileSelection(IGridTile gridTile) => OnHandleTileSelection?.Invoke(gridTile);
    
}
