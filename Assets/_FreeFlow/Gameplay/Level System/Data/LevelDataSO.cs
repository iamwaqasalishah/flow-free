using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "LevelData")]
public class LevelDataSO : ScriptableObject
{
    public static LevelDataSO Default => Resources.Load<LevelDataSO>(nameof(LevelDataSO));

    [SerializeField] private List<LevelSO> _levels = new List<LevelSO>();

    public LevelData GetLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= _levels.Count)
            return null;
        
        return _levels[levelIndex].levelData;
    }

    public int GetNumberOfLevels()
    {
        return _levels.Count;
    }
}

