using System;
using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core.Parser.Analysis;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelDataSO : ScriptableObject
{
    public static LevelDataSO Default => Resources.Load<LevelDataSO>(nameof(LevelDataSO));

    [SerializeField] private List<LevelData> _levelsData = new List<LevelData>();

    public LevelData GetLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _levelsData.Count)
            return null;
        
        return _levelsData[levelIndex];
    }
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