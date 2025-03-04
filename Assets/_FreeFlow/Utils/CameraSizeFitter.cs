using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeFitter : MonoBehaviour
{
    [SerializeField] private float _worldGridSize=5 ;

    [SerializeField] private Mode _mode;

    private Camera _camera;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _camera = Camera.main;
        if (_mode == Mode.VerticalFit)
        {
            _camera.orthographicSize = _worldGridSize / 2;
        }
        else
        {
            var aspectRatio = (float)Screen.height / Screen.width;
            Debug.Log(aspectRatio);
            _camera.orthographicSize = (aspectRatio * _worldGridSize ) / 2f;
            
        }

    }


    private enum Mode
    {
        VerticalFit,
        HorizontalFit
    }
}
