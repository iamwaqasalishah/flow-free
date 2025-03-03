using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GridSettingConfigs", menuName = "Grid Setting")]
public class GridSettingConfigs : ScriptableObject
{
   public static GridSettingConfigs Default => Resources.Load<GridSettingConfigs>("GridSettingConfigs");
   
   [HeaderAttribute("Grid Settings")]
   public float GridSize = 5;
   public float TileSpacing = 0.05f;
   public GridTile GridTilePrefab;
}
