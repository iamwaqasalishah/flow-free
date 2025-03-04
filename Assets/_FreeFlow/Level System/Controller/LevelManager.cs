using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private const string LevelKey = "CurrentLevel"; 
    [SerializeField] private GridController _gridController;
    [SerializeField] private int totalLevels = 5; 

    private int currentLevel;

    private void Start()
    {
        currentLevel = PlayerPrefs.GetInt(LevelKey, 1);
        LoadLevel(currentLevel);
    }

    private void LoadLevel(int levelToLoad)
    {
        if (LevelDataSO.Default == null)return;
        
        int levelIndex = ((levelToLoad - 1) % totalLevels) + 1;

        var level = LevelDataSO.Default.GetLevel(levelIndex);

        if (level != null)
        {
            _gridController.Init(level.GridSize, level);
        }
        
    }

    public void LoadNextLevel()
    {
        currentLevel++;
        PlayerPrefs.SetInt(LevelKey, currentLevel);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
}