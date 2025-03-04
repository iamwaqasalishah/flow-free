
using System.Linq;
using UnityEngine;

public class GridController : GridBase
{
    public void Init(int gridSize,LevelData levelData)
    {
        SetUp(gridSize);
        var paths= levelData.Paths;
        foreach (var data in paths)
        {
            GridTile startTile=GetTileByIndex(data.Points.First().x, data.Points.First().y);
            GridTile endTile = GetTileByIndex(data.Points.Last().x, data.Points.Last().y);
           SetTilesData(startTile,endTile,data.Color);
          
        }
    }

    private void SetTilesData(GridTile startTile,GridTile endTile,ColorType color)
    {
        startTile.IsNode=true;
        endTile.IsNode=true;
        startTile.Color = color;
        endTile.Color = color;
    }
    
}