using System;

public static partial class EventManager 
{
    public static Action OnLoadGame;
    public static void DoFireOnLoadGame() => OnLoadGame?.Invoke();
    
    public static Action OnStartGame;
    public static void DoFireOnGameStart() => OnStartGame?.Invoke();
}
