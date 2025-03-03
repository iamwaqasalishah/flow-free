// /*
// Created by Darsan
// */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ColorGroup", fileName = "ColorData")]
public class ColorGroupSO : ScriptableObject
{
    public static ColorGroupSO Default => Resources.Load<ColorGroupSO>(nameof(ColorGroupSO));

    [SerializeField] private List<Color> _colors = new List<Color>();

    public Color GetColor(int index)
    {
        return _colors[index];
    }   

}