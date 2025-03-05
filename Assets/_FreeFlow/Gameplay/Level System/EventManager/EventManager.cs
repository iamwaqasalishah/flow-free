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
    
    public static event Action OnLevelNumberUpdate;
    public static void DoFireOnLevelNumberUpdate() => OnLevelNumberUpdate?.Invoke();

    public static event Action<int> OnSetPathsCount;
    public static void DoFireOnSetPathsCount(int value) => OnSetPathsCount?.Invoke(value);
}
