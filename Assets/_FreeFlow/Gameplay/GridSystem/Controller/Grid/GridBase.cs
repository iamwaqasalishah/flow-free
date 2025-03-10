using System;
using System.Collections.Generic;
using UnityEngine;


public class GridBase : MonoBehaviour
{
    [Header("Grid Settings")]
    private GridTile _gridTilePrefab;
    private float _width;
    private float _spacing;

    private Dictionary<Vector2Int, GridTile> _tiles = new Dictionary<Vector2Int, GridTile>();
    
    protected virtual void LoadGridData()
    {
        _width = GridSettingConfigs.Default.GridSize; 
        _spacing = GridSettingConfigs.Default.TileSpacing; 
        _gridTilePrefab = GridSettingConfigs.Default.GridTilePrefab; 
    }

    protected void SetUp(int count)
    {
        float tileSize = (_width - (count - 1) * _spacing) / count; 
        Vector3 lowerLeft = transform.position - new Vector3(1, 1, 0) * _width / 2; 

        for (int row = 0; row < count; row++)
        {
            for (int col = 0; col < count; col++)
            {
                Vector2Int coordinate = new Vector2Int(col, row); 

                GridTile gridTile = Instantiate(_gridTilePrefab, transform);
                gridTile.Coordinate = coordinate;
                gridTile.Size = tileSize;
                gridTile.name = $"Tile_{col}_{row}"; 

                gridTile.transform.position = lowerLeft +
                    ((row + 0.5f) * tileSize + row * _spacing) * Vector3.up +  
                    ((col + 0.5f) * tileSize + col * _spacing) * Vector3.right; 
                
                _tiles[coordinate] = gridTile;
            }
        }
        AssignNeighbors();
    }
    private void AssignNeighbors()
    {
        int[] dx = { 0, 0, -1, 1 }; 
        int[] dy = { -1, 1, 0, 0 }; 

        foreach (var tile in _tiles.Values)
        {
            List<IGridTile> neighbors = new List<IGridTile>();

            for (int i = 0; i < 4; i++) 
            {
                Vector2Int neighborCoord = new Vector2Int(tile.Coordinate.x + dx[i], tile.Coordinate.y + dy[i]);

                if (_tiles.TryGetValue(neighborCoord, out GridTile neighbor))
                {
                    neighbors.Add(neighbor);
                }
            }

            tile.SetNeighbors(neighbors); 
        }
    }
    public GridTile GetTileByIndex(int x, int y)
    {
        _tiles.TryGetValue(new Vector2Int(x, y), out GridTile tile);
        return tile; 
    }
}