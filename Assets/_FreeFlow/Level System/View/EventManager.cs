using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Level Complete Events
public static partial class EventManager
{
    public static event Action OnEnableLevelCompletePanel;
    public static void DoFireOnEnableLevelCompletePanel() => OnEnableLevelCompletePanel?.Invoke();
    
    public static event Action OnRestartLevel;
    public static void DoFireOnRestartLevel() => OnRestartLevel?.Invoke();
    
    public static event Action OnNextLevel;
    public static void DoFireOnNextLevel() => OnNextLevel?.Invoke();
}
