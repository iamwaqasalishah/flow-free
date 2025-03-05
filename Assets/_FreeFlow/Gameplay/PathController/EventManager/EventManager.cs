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
}
