using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private int _totalLevels = 5;
    private int _totalPaths;

    private void OnEnable()
    {
        EventManager.OnStartGame += LoadData;
        EventManager.OnRestartLevel += RestartLevel;
        EventManager.OnNextLevel += LoadNextLevel;
        EventManager.OnUpdatePathsCount += UpdatePathsCount;
    }

    private void OnDisable()
    {
        EventManager.OnStartGame -= LoadData;
        EventManager.OnRestartLevel -= RestartLevel;
        EventManager.OnNextLevel -= LoadNextLevel;
        EventManager.OnUpdatePathsCount -= UpdatePathsCount;
    }

    private void LoadData()
    {
        var _currentLevel = DB.LevelNumber;

        _totalLevels = LevelDataSO.Default.GetNumberOfLevels();
        LoadLevel(_currentLevel);
    }

    private void LoadLevel(int levelToLoad)
    {
        int levelIndex = ((levelToLoad - 1) % _totalLevels) + 1;

        LevelData level = LevelDataSO.Default.GetLevel(levelIndex - 1);
        if (level != null)
        {
            EventManager.DoFireOnInitializeGrid(level.GridSize, level);
            _totalPaths = level.Paths.Count;
        }
    }

    private void UpdatePathsCount(int value)
    {
        if (value >= _totalPaths)
        {
            LevelComplete();
        }
    }

    private void LevelComplete()
    {
        EventManager.DoFireOnDisableInteraction();
        EventManager.DoFireOnEnableLevelCompletePanel();
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