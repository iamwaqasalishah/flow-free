// /*
// Created by Darsan
// */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "ColorGroup", fileName = "ColorData")]
public class ColorGroupSO : ScriptableObject
{
    public static ColorGroupSO Default => Resources.Load<ColorGroupSO>(nameof(ColorGroupSO));

    //[SerializeField] private List<Color> _colors = new List<Color>();
    [SerializeField] private List<ColorData> _colorsData = new List<ColorData>();
   // [SerializeField] private Color _defaultColor;
    //public Color GetDefaultColor() => _defaultColor;

    public Color GetColor(ColorType type)
    {
        foreach (var colorData in _colorsData)
        {
            if (type == colorData.ColorType)
            {
                return colorData.Color;
            }
        }

        return _colorsData[0].Color;
    }
}

[Serializable]
public class ColorData
{
    public ColorType ColorType;
    public Color Color;
}

public enum ColorType
{
    None,
    Red,
    Pink,
    Yellow,
    Purple,
    Blue,
    SkyBlue,
    LightGreen,
    Orange,
    Green
}