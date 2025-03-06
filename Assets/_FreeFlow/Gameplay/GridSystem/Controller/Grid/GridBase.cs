using System;
using System.Collections.Generic;
using UnityEngine;


public class GridBase : MonoBehaviour
{
    [Header("Grid Settings")]
    private GridTile gridTilePrefab;
    private float width;
    private float spacing;

    protected Dictionary<Vector2Int, GridTile> _tiles = new Dictionary<Vector2Int, GridTile>();

    private void Awake()
    {
        LoadGridData();
    }

   

    private void LoadGridData()
    {
        width = GridSettingConfigs.Default.GridSize; 
        spacing = GridSettingConfigs.Default.TileSpacing; 
        gridTilePrefab = GridSettingConfigs.Default.GridTilePrefab; 
    }

    protected void SetUp(int count)
    {
        float tileSize = (width - (count - 1) * spacing) / count; 
        Vector3 lowerLeft = transform.position - new Vector3(1, 1, 0) * width / 2; 

        for (int row = 0; row < count; row++)
        {
            for (int col = 0; col < count; col++)
            {
                Vector2Int coordinate = new Vector2Int(col, row); 

                GridTile gridTile = Instantiate(gridTilePrefab, transform);
                gridTile.Coordinate = coordinate;
                gridTile.Size = tileSize;
                gridTile.name = $"Tile_{col}_{row}"; 

                gridTile.transform.position = lowerLeft +
                    ((row + 0.5f) * tileSize + row * spacing) * Vector3.up +  
                    ((col + 0.5f) * tileSize + col * spacing) * Vector3.right; 

                
                _tiles[coordinate] = gridTile;
            }
        }
    }

    public GridTile GetTileByIndex(int x, int y)
    {
        _tiles.TryGetValue(new Vector2Int(x, y), out GridTile tile);
        return tile; 
    }
}