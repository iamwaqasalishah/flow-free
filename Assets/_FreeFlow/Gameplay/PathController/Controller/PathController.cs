using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private Dictionary<ColorType, List<GridTile>> _paths = new Dictionary<ColorType, List<GridTile>>();
    private Stack<ColorType> _undoStack = new Stack<ColorType>();
    private List<GridTile> _currentSelection;
    private ColorType _currentColor;
    private int _totalNumberOfPaths;

    [SerializeField] GridController _gridController;
    [SerializeField] private PathVisualizer _pathVisualizer;

    private void OnEnable()
    {
        EventManager.OnSetPathsCount += OnSetPathsCount;
        EventManager.OnUndo += UndoLastPath;
        EventManager.OnReset += ResetAllPaths;
    }

    private void OnDisable()
    {
        EventManager.OnUndo -= UndoLastPath;
        EventManager.OnReset -= ResetAllPaths;
        EventManager.OnSetPathsCount -= OnSetPathsCount;
    }

    private void Awake()
    {
    }

    private void OnSetPathsCount(int count)
    {
        _totalNumberOfPaths = count;
    }

    public void StartNewPath(GridTile gridTile)
    {
        _currentColor = gridTile.Color;

        if (_paths.ContainsKey(_currentColor))
        {
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

        if (_currentSelection.Count > 1 && _currentSelection[_currentSelection.Count - 2] == gridTile)
        {
            Backtrack();
            return;
        }

        if (lastTile.IsNode && _currentSelection.Count > 1) return;

        foreach (var path in _paths)
        {
            if (path.Key != _currentColor && path.Value.Contains(gridTile))
            {
                ResetPath(path.Key);
                break;
            }
        }

        if (_currentSelection.Contains(gridTile)) return;
        if (gridTile.IsNode && gridTile.Color != _currentColor) return;

        // If it's adjacent, add directly
        if (IsAdjacent(lastTile, gridTile))
        {
            _currentSelection.Add(gridTile);
            gridTile.Color = _currentColor;
            _pathVisualizer.AddPoint(_currentColor, gridTile.transform.position);
        }
        else
        {
            // Only allow if there is a valid best path with an adjacent step
            List<GridTile> bestPath = FindBestPath(lastTile, gridTile);
            if (bestPath == null) return;

            foreach (var tile in bestPath)
            {
                _currentSelection.Add(tile);
                tile.Color = _currentColor;
                _pathVisualizer.AddPoint(_currentColor, tile.transform.position);
            }
        }
    }

    private List<GridTile> GetNeighbors(GridTile tile)
    {
        List<GridTile> neighbors = new List<GridTile>();

        int[] dx = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dy = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            GridTile neighbor = _gridController.GetTileByIndex(tile.Coordinate.x + dx[i], tile.Coordinate.y + dy[i]);
            if (neighbor != null) neighbors.Add(neighbor);
        }

        return neighbors;
    }

    private List<GridTile> FindBestPath(GridTile start, GridTile target)
    {
        Queue<List<GridTile>> queue = new Queue<List<GridTile>>();
        HashSet<GridTile> visited = new HashSet<GridTile>();

        queue.Enqueue(new List<GridTile> { start });
        visited.Add(start);

        while (queue.Count > 0)
        {
            List<GridTile> path = queue.Dequeue();
            GridTile lastTile = path[path.Count - 1];

            if (lastTile == target) 
            {
                // Ensure the path only takes an adjacent step before diagonal
                if (path.Count == 2) return path; 
                else return null; // Ignore direct diagonal moves
            }

            foreach (var neighbor in GetNeighbors(lastTile))
            {
                if (visited.Contains(neighbor)) continue;
                if (neighbor.IsNode && neighbor.Color != _currentColor) continue;
            
                // Avoid direct diagonal jumps
                if (!IsAdjacent(lastTile, neighbor)) continue; 

                List<GridTile> newPath = new List<GridTile>(path) { neighbor };
                queue.Enqueue(newPath);
                visited.Add(neighbor);
            }
        }
        return null;
        
    }

    private bool IsAdjacent(GridTile a, GridTile b)
    {
        return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) + Mathf.Abs(a.Coordinate.y - b.Coordinate.y) == 1;
    }

    private bool IsAdjacentOrDiagonal(GridTile a, GridTile b)
    {
        int dx = Mathf.Abs(a.Coordinate.x - b.Coordinate.x);
        int dy = Mathf.Abs(a.Coordinate.y - b.Coordinate.y);

        return (dx <= 1 && dy <= 1) && (dx + dy > 0);
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
            CheckLevelComplete();
        }
        else
        {
            _pathVisualizer.ClearLine(_currentColor);
            ResetPath(_currentColor);
            _currentSelection = null;
        }
    }

    private bool IsAllPathsCompleted()
    {
        return _paths.Count == _totalNumberOfPaths;
    }

    private void CheckLevelComplete()
    {
        if (IsAllPathsCompleted())
        {
            EventManager.DoFireOnDisableInteraction();
            EventManager.DoFireOnEnableLevelCompletePanel();
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
        ResetPath(lastColor);
    }
}