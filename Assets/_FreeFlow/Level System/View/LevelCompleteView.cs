using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCompleteView : MonoBehaviour
{
    [SerializeField] private LevelCompleteViewRefs _levelCompleteViewRefs;

    private void OnEnable()
    {
        EventManager.OnEnableLevelCompletePanel += EnableView;
    }

    private void OnDisable()
    {
        EventManager.OnEnableLevelCompletePanel -= EnableView;
    }

    private void Start()
    {
        SetUpButtonListeners();
    }

    private void EnableView()
    {
        _levelCompleteViewRefs.Panel.SetActive(true);
    }
    private void SetUpButtonListeners()
    {
        _levelCompleteViewRefs.NextLevelButton.onClick.AddListener(OnContinueButtonTapped);
        _levelCompleteViewRefs.RestartLevelButton.onClick.AddListener(OnRestartButtonTapped);
    }

    private void OnRestartButtonTapped()
    {
        EventManager.DoFireOnRestartLevel();
    }
    private void OnContinueButtonTapped()
    {
        EventManager.DoFireOnNextLevel();
    }
    
}
