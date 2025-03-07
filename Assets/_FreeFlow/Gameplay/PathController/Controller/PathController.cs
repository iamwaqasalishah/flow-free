using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathController : MonoBehaviour
{
  
    private Dictionary<ColorType, List<IGridTile>> _paths = new Dictionary<ColorType, List<IGridTile>>();
    private List<IGridTile> _currentSelection;
    private ColorType _currentColor;
    private bool IsDiagonal(IGridTile a, IGridTile b) => Mathf.Abs(a.Coordinate.x - b.Coordinate.x) == 1 && Mathf.Abs(a.Coordinate.y - b.Coordinate.y) == 1;
    private bool IsAdjacent(IGridTile a, IGridTile b) => Mathf.Abs(a.Coordinate.x - b.Coordinate.x) + Mathf.Abs(a.Coordinate.y - b.Coordinate.y) == 1;

    [SerializeField] private PathVisualizer _pathVisualizer;

    private void OnEnable()
    {
        SubscribeToEvents();
    }
    
    private void OnDisable()
    {
        UnsubscribeFromEvents();
        
    } 

    private void SubscribeToEvents()
    {
        EventManager.OnUndo += UndoLastPath;
        EventManager.OnReset += ResetAllPaths;
        EventManager.OnHandleTileSelection += HandleTileSelection;
        EventManager.OnStartNewPath += StartNewPath;
        EventManager.OnValidatePath += ValidatePath;
    }

    private void UnsubscribeFromEvents()
    {
        EventManager.OnUndo -= UndoLastPath;
        EventManager.OnReset -= ResetAllPaths;
        EventManager.OnHandleTileSelection -= HandleTileSelection;
        EventManager.OnStartNewPath -= StartNewPath;
        EventManager.OnValidatePath -= ValidatePath;
    }

    public void StartNewPath(IGridTile gridTile)
    {
        _currentColor = gridTile.Color;
        ResetExistingPath(_currentColor);
        InitializeNewPath(gridTile);
    }

    private void ResetExistingPath(ColorType color)
    {
        if (_paths.ContainsKey(color))
        {
            ResetPath(color);
        }
    }

    private void InitializeNewPath(IGridTile gridTile)
    {
        _currentSelection = new List<IGridTile> { gridTile };
        gridTile.Color = _currentColor;

        if (gridTile is GridTile tile)
        {
            _pathVisualizer.CreateLineRenderer(_currentColor, tile.transform.position);
        }
    }

    public void HandleTileSelection(IGridTile gridTile)
    {
        if (_currentSelection == null) return;
        if (IsBacktracking(gridTile)) return;

        IGridTile lastTile = _currentSelection[^1];
        if (lastTile.IsNode && _currentSelection.Count > 1) return;
        if (gridTile.IsNode && gridTile.Color != _currentColor) return;

        ProcessTileSelection(gridTile, lastTile);
    }

    private bool IsBacktracking(IGridTile gridTile)
    {
        int existingIndex = _currentSelection.IndexOf(gridTile);
        if (existingIndex != -1)
        {
            TrimPath(existingIndex);
            return true;
        }
        return false;
    }

    private void ProcessTileSelection(IGridTile currentTile, IGridTile lastTile)
    {
        List<IGridTile> bestPath = GetValidPath(lastTile, currentTile);
        if (bestPath == null) return;
        AppendPath(bestPath);
        ResetIntersectingPaths(currentTile);
       
    }

    private List<IGridTile> GetValidPath(IGridTile start, IGridTile target)
    {
        List<IGridTile> bestPath = FindBestPath(start, target);
        return bestPath != null ? IsPathFullyAdjacent(start, bestPath) : null;
    }

    private void ResetIntersectingPaths(IGridTile gridTile)
    {
        if (gridTile.IsNode) return; 

        List<ColorType> pathsToReset = new List<ColorType>();

        foreach (var path in _paths)
        {
            if (path.Key == _currentColor) continue; 

            HashSet<IGridTile> pathTiles = new HashSet<IGridTile>(path.Value); 

            bool intersectsWithCurrentSelection = false;

            foreach (var tile in _currentSelection)
            {
                if (pathTiles.Contains(tile)) 
                {
                    intersectsWithCurrentSelection = true;
                }
                
                if (intersectsWithCurrentSelection )
                {
                    pathsToReset.Add(path.Key);
                    break; 
                }
            }
        }

        foreach (ColorType color in pathsToReset)
        {
            ResetPath(color);
        }
    }

    private void AppendPath(List<IGridTile> bestPath)
    {
        foreach (var tile in bestPath)
        {
            if (_currentSelection.Contains(tile)) continue;
            if (tile.IsNode && tile.Color != _currentColor) continue;

            _currentSelection.Add(tile);
            tile.Color = _currentColor;

            if (tile is GridTile concreteTile)
            {
                _pathVisualizer.AddPoint(_currentColor, concreteTile.transform.position);
            }
        }
    }

    private void TrimPath(int index)
    {
        for (int i = _currentSelection.Count - 1; i > index; i--)
        {
            IGridTile tile = _currentSelection[i];
            if (!tile.IsNode) tile.Color = ColorType.None;
            _pathVisualizer.RemoveLastPoint(_currentColor);
            _currentSelection.RemoveAt(i);
        }
    }

    private List<IGridTile> IsPathFullyAdjacent(IGridTile lastTile, List<IGridTile> path)
    {
        List<IGridTile> filteredPath = new List<IGridTile>();
        IGridTile previousTile = lastTile;

        foreach (var tile in path)
        {
            if (IsAdjacent(lastTile, tile) && (filteredPath.Count == 0 || IsAdjacent(previousTile, tile)))
            {
                filteredPath.Add(tile);
                previousTile = tile;
            }
        }

        return filteredPath;
    }

    public void ValidatePath()
    {
        if (_currentSelection != null && _currentSelection.Count > 1 && _currentSelection[^1].IsNode)
        {
            _paths[_currentColor] = new List<IGridTile>(_currentSelection);
            _currentColor = ColorType.None;
            _currentSelection = null;
            EventManager.DoFireOnUpdatePathsCount(_paths.Count);
        }
        else
        {
            _pathVisualizer.RemoveLineRenderer(_currentColor);
            ResetPath(_currentColor);
            _currentSelection = null;
        }
    }

    private List<IGridTile> FindBestPath(IGridTile start, IGridTile target)
    {
        Queue<IGridTile> queue = new Queue<IGridTile>();
        Dictionary<IGridTile, IGridTile> cameFromParent = new Dictionary<IGridTile, IGridTile>();
        HashSet<IGridTile> visited = new HashSet<IGridTile>();

        queue.Enqueue(start);
        visited.Add(start);
        cameFromParent[start] = null;

        while (queue.Count > 0)
        {
            IGridTile current = queue.Dequeue();

            if (current == target)
            {
                return ConstructPath(cameFromParent, current);
            }

            foreach (var neighbor in current.Neighbors)
            {
                if (visited.Contains(neighbor) || IsDiagonal(current, neighbor)) continue;
                if (neighbor.IsNode && neighbor.Color != _currentColor) continue;

                queue.Enqueue(neighbor);
                visited.Add(neighbor);
                cameFromParent[neighbor] = current;
            }
        }

        return null;
    }

    private List<IGridTile> ConstructPath(Dictionary<IGridTile, IGridTile> cameFrom, IGridTile target)
    {
        List<IGridTile> path = new List<IGridTile>();

        while (target != null)
        {
            if (path.Count > 0 && IsDiagonal(path[^1], target))
            {
                target = cameFrom[target];
                continue;
            }

            path.Add(target);
            target = cameFrom[target];
        }

        path.Reverse();
        return path;
    }

    private void UndoLastPath()
    {
        if (_paths.Count == 0) return;  
        ColorType lastColor = _paths.Keys.Last();
        ResetPath(lastColor);
    }

    private void ResetPath(ColorType color)
    {
        if (!_paths.ContainsKey(color)) return;

        foreach (var tile in _paths[color])
        {
            if (!tile.IsNode) tile.Color = ColorType.None;
        }

        _paths.Remove(color);
        _pathVisualizer.RemoveLineRenderer(color);
    }

    private void ResetAllPaths()
    {
        foreach (var color in _paths.Keys)
        {
            foreach (var tile in _paths[color])
            {
                if (!tile.IsNode) tile.Color = ColorType.None;
            }
        }

        _paths.Clear();
        _pathVisualizer.ClearAllLines();
    }

    
}