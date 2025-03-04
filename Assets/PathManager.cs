using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    private Dictionary<ColorType, List<GridTile>> _paths = new();
    private ColorType _currentColor;
    public List<GridTile> _currentSelection;

    private void OnEnable()
    {
        EventManager.OnStartPath += StartPath;
        EventManager.OnEndPath += EndPath;
        EventManager.OnTileAdded += AddTileToPath;
    }

    private void OnDisable()
    {
        EventManager.OnStartPath -= StartPath;
        EventManager.OnEndPath -= EndPath;
        EventManager.OnTileAdded -= AddTileToPath;
    }

    private void StartPath(GridTile startTile)
    {
        if (!startTile.IsNode) return;

        _currentColor = startTile.Color;
        _currentSelection = _paths.ContainsKey(_currentColor)
            ? new List<GridTile>(_paths[_currentColor])
            : new List<GridTile>();

        _currentSelection.Add(startTile);
        startTile.Color = _currentColor;

        //EventManager.DoFireOnStartPath(startTile);
    }

    private void AddTileToPath(GridTile tile)
    {
        if (_currentSelection == null || !IsValidMove(tile)) return;

        if (_currentSelection.Contains(tile) || (tile.IsNode && tile.Color != _currentColor)) return;

        _currentSelection.Add(tile);
        tile.Color = _currentColor;

        //EventManager.DoFireOnTileAdded(tile);
    }

    private bool IsValidMove(GridTile tile)
    {
        GridTile lastTile = _currentSelection[^1];
        return Mathf.Abs(lastTile.Coordinate.x - tile.Coordinate.x) +
            Mathf.Abs(lastTile.Coordinate.y - tile.Coordinate.y) == 1;
    }

    public void EndPath()
    {
        if (_currentSelection.Count > 1)
        {
            _paths[_currentColor] = new List<GridTile>(_currentSelection);
        }
        else
        {
            ResetPath(_currentColor);
        }

       // EventManager.DoFireOnEndPath();
    }

    private void ResetPath(ColorType color)
    {
        if (!_paths.ContainsKey(color)) return;

        foreach (var tile in _paths[color])
        {
            if (!tile.IsNode) tile.Color = ColorType.None;
        }

        _paths.Remove(color);
    }
}