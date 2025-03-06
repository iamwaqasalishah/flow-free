using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "LevelData")]
public class LevelSO : ScriptableObject
{
   public LevelData levelData;
}
[Serializable]
public class LevelData
{
    public int GridSize;
    public List<PathData> Paths;
}

[Serializable]
public class PathData
{
    public List<Vector2Int> Points;
    public ColorType Color;
}