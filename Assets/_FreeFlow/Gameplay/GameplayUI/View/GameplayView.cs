using System;
using System.Collections;
using System.Collections.Generic;
using RDG;
using UnityEngine;
using UnityEngine.Serialization;

public class GameplayView : MonoBehaviour
{
    [SerializeField] private GameplayViewRefs _gameplayViewRefs;

    private void Awake()
    {
        Application.targetFrameRate = 90;
    }

    private void OnEnable()
    {
       
    }

    private void OnDisable()
    {
        
    }

    private void Start()
    {
        SetUpButtonListeners();
    }

    

    private void SetUpButtonListeners()
    {
        _gameplayViewRefs.UndoButton.onClick.AddListener(OnUndoButtonTapped);
        _gameplayViewRefs.ResetButton.onClick.AddListener(OnResetButtonTapped);
    }

    private void OnResetButtonTapped()
    {
        Vibration.Vibrate(10);
        EventManager.DoFireOnReset();
    }

    private void OnUndoButtonTapped()
    {
        Vibration.Vibrate(10);
        EventManager.DoFireOnUndo();
    }
}
