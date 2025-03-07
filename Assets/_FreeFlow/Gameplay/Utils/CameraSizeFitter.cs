using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSizeFitter : MonoBehaviour
{
    [SerializeField] private float _worldGridSize = 5;


    private Camera _camera;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _camera = Camera.main;
        var aspectRatio = (float)Screen.height / Screen.width;
        _camera.orthographicSize = (aspectRatio * _worldGridSize) / 2f;
    }
}