using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class PathController : MonoBehaviour
{
    [ShowInInspector]
    private Dictionary<ColorType, List<GridTile>> _paths = new Dictionary<ColorType, List<GridTile>>();

    [ShowInInspector] private Stack<ColorType> _undoStack = new Stack<ColorType>();
    [ShowInInspector] private List<GridTile> _currentSelection;
    private ColorType _currentColor;
    private int _totalNumberOfPaths;

    [SerializeField] private PathVisualizer _pathVisualizer;

    private void OnEnable()
    {
        EventManager.OnUndo += UndoLastPath;
        EventManager.OnReset += ResetAllPaths;
        EventManager.OnHandleTileSelection += HandleTileSelection;
        EventManager.OnStartNewPath += StartNewPath;
        EventManager.OnValidatePath += ValidatePath;
    }

    private void OnDisable()
    {
        EventManager.OnUndo -= UndoLastPath;
        EventManager.OnReset -= ResetAllPaths;
        EventManager.OnHandleTileSelection -= HandleTileSelection;
        EventManager.OnStartNewPath -= StartNewPath;
        EventManager.OnValidatePath -= ValidatePath;
    }
    
    public void StartNewPath(GridTile gridTile)
    {
        _currentColor = gridTile.Color;

        if (_paths.ContainsKey(_currentColor))
        {
            Debug.Log("starting new path");
            ResetPath(_currentColor);
        }

        _currentSelection = new List<GridTile> { gridTile };

        gridTile.Color = _currentColor;

        _pathVisualizer.CreateLineRenderer(_currentColor, gridTile.transform.position);
    }

    public void HandleTileSelection(GridTile gridTile)
    {
        if (_currentSelection == null) return;

        GridTile lastTile = _currentSelection[_currentSelection.Count - 1];

        // 🚨 **Check if the selected tile is already in `_currentSelection`**
        int existingIndex = _currentSelection.IndexOf(gridTile);
        if (existingIndex != -1)
        {
            // ✅ **Trim the path to only keep tiles up to the selected tile**
            TrimPath(existingIndex);
            return;
        }

        if (_currentSelection.Count > 1 && _currentSelection[_currentSelection.Count - 2] == gridTile)
        {
            Backtrack();
            return;
        }

        if (lastTile.IsNode && _currentSelection.Count > 1) return;

        // 🚨 **Check if the selected tile is part of another path**
        ColorType previousPathColor = ColorType.None;
        bool isIntersectingPath = false;

        foreach (var path in _paths)
        {
            if (path.Value.Contains(gridTile))
            {
                previousPathColor = path.Key;
                isIntersectingPath = true;
                break;
            }
        }

        if (gridTile.IsNode && gridTile.Color != _currentColor) return;

        // ✅ **Step 1: Find a valid path before resetting anything**
        List<GridTile> bestPath = FindBestPath(lastTile, gridTile);
        if (bestPath == null) return; // ❌ No valid path → Do NOT reset anything

        bestPath = IsPathFullyAdjacent(lastTile, bestPath);

        // ✅ **Step 2: Remove previous path ONLY if a valid path exists**
        if (isIntersectingPath && previousPathColor != ColorType.None)
        {
            Debug.Log("✅ Resetting previous path color because a valid path exists.");
            ResetPath(previousPathColor);
        }

        foreach (var tile in bestPath)
        {
            if (_currentSelection.Contains(tile)) continue;
            if (tile.IsNode && tile.Color != _currentColor) continue;

            _currentSelection.Add(tile);
            tile.Color = _currentColor;
            _pathVisualizer.AddPoint(_currentColor, tile.transform.position);
        }
    }

    private void TrimPath(int index)
    {
        // ✅ **Remove all tiles after the selected tile**
        for (int i = _currentSelection.Count - 1; i > index; i--)
        {
            GridTile tile = _currentSelection[i];
            if (!tile.IsNode) tile.Color = ColorType.None;
            _pathVisualizer.RemoveLastPoint(_currentColor);
            _currentSelection.RemoveAt(i);
        }
    }

    private List<GridTile> IsPathFullyAdjacent(GridTile lastTile, List<GridTile> path)
    {
        List<GridTile> filteredPath = new List<GridTile>();
        GridTile previousTile = lastTile;

        foreach (var tile in path)
        {
            // ✅ Ensure tile is adjacent to `lastTile` and previous tile in the path
            if (IsAdjacent(lastTile, tile) && (filteredPath.Count == 0 || IsAdjacent(previousTile, tile)))
            {
                filteredPath.Add(tile);
                previousTile = tile; // Update previous tile
            }
        }

        return filteredPath;
    }

    private List<GridTile> GetNeighbors(GridTile tile)
    {
        List<GridTile> neighbors = new List<GridTile>();

        int[] dx = { 0, 0, -1, 1 };
        int[] dy = { -1, 1, 0, 0 };

        for (int i = 0; i < 4; i++)
        {
            GridTile neighbor =
                EventManager.DoFireOnGetTileByIndex(tile.Coordinate.x + dx[i], tile.Coordinate.y + dy[i]);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    private List<GridTile> FindBestPath(GridTile start, GridTile target)
    {
        Queue<GridTile> queue = new Queue<GridTile>();
        Dictionary<GridTile, GridTile> cameFrom = new Dictionary<GridTile, GridTile>();
        HashSet<GridTile> visited = new HashSet<GridTile>();

        queue.Enqueue(start);
        visited.Add(start);
        cameFrom[start] = null;

        while (queue.Count > 0)
        {
            GridTile current = queue.Dequeue();

            if (current == target)
            {
                List<GridTile> path = new List<GridTile>();
                while (current != null)
                {
                    // ✅ **Remove diagonal tiles before adding to path**
                    if (path.Count > 0 && IsDiagonal(path[path.Count - 1], current))
                    {
                        current = cameFrom[current]; // ❌ Skip diagonal tile
                        continue;
                    }

                    path.Add(current);
                    current = cameFrom[current];
                }

                path.Reverse();
                return path;
            }

            foreach (var neighbor in GetNeighbors(current))
            {
                if (visited.Contains(neighbor)) continue;
                if (neighbor.IsNode && neighbor.Color != _currentColor) continue;

                // 🚨 **Reject diagonals with current tile**
                if (IsDiagonal(current, neighbor)) continue;

                queue.Enqueue(neighbor);
                visited.Add(neighbor);
                cameFrom[neighbor] = current;
            }
        }

        return null; // No valid path found
    }

    private bool IsDiagonal(GridTile a, GridTile b)
    {
        int dx = Mathf.Abs(a.Coordinate.x - b.Coordinate.x);
        int dy = Mathf.Abs(a.Coordinate.y - b.Coordinate.y);

        return (dx == 1 && dy == 1); // ❌ True if diagonal
    }

    private bool IsAdjacent(GridTile a, GridTile b)
    {
        Vector2Int coordA = a.Coordinate;
        Vector2Int coordB = b.Coordinate;

        int dx = Mathf.Abs(coordA.x - coordB.x);
        int dy = Mathf.Abs(coordA.y - coordB.y);

        return (dx == 1 && dy == 0) || (dx == 0 && dy == 1);
    }

    private void Backtrack()
    {
        GridTile lastTile = _currentSelection[_currentSelection.Count - 1];
        if (!lastTile.IsNode) lastTile.Color = ColorType.None;

        _currentSelection.RemoveAt(_currentSelection.Count - 1);

        _pathVisualizer.RemoveLastPoint(_currentColor);
    }

    public void ValidatePath()
    {
        if (_currentSelection != null && _currentSelection.Count > 1 &&
            _currentSelection[_currentSelection.Count - 1].IsNode)
        {
            _paths[_currentColor] = new List<GridTile>(_currentSelection);
            _undoStack.Push(_currentColor);
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

    public void ResetAllPaths()
    {
        foreach (var color in _paths.Keys)
        {
            foreach (var tile in _paths[color])
            {
                if (!tile.IsNode) tile.Color = ColorType.None;
            }
        }

        _paths.Clear();
        _undoStack.Clear();
        _pathVisualizer.ClearAllLines();
    }


    public void UndoLastPath()
    {
        if (_undoStack.Count == 0) return;

        ColorType lastColor = _undoStack.Pop();
        Debug.Log("undo last path");
        ResetPath(lastColor);
    }
}