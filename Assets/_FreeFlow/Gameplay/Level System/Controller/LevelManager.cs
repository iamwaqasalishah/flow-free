using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int _totalLevels = 5;
    private void OnEnable()
    {
        EventManager.OnRestartLevel += RestartLevel;
        EventManager.OnNextLevel += LoadNextLevel;
    }

    private void OnDisable()
    {
        EventManager.OnRestartLevel -= RestartLevel;
        EventManager.OnNextLevel -= LoadNextLevel;
    }

    private void Start()
    {
        var _currentLevel = DB.LevelNumber;

        if (LevelDataSO.Default == null) return;
        _totalLevels = LevelDataSO.Default.GetNumberOfLevels();
        LoadLevel(_currentLevel);
    }

    private void LoadLevel(int levelToLoad)
    {
        int levelIndex = ((levelToLoad - 1) % _totalLevels) + 1;
       
        LevelData level = LevelDataSO.Default.GetLevel(levelIndex-1);
        if (level != null)
        {
            EventManager.DoFireOnInitializeGrid(level.GridSize, level);
            EventManager.DoFireOnSetPathsCount(level.Paths.Count);
        }
    }

    public void LoadNextLevel()
    {
        DB.LevelNumber++;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}