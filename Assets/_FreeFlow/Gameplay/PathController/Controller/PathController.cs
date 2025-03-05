using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
    private Dictionary<ColorType, List<GridTile>> _paths = new Dictionary<ColorType, List<GridTile>>();
    private Dictionary<ColorType, HashSet<GridTile>> _activeTiles = new Dictionary<ColorType, HashSet<GridTile>>();
    
    private List<GridTile> _currentSelection;
    private HashSet<GridTile> _currentActiveTiles;
    private ColorType _currentColor;
    private int _totalNumberOfPaths;

    [SerializeField] private PathVisualizer _pathVisualizer;
    private Stack<ColorType> _undoStack = new Stack<ColorType>(); // Stack for undoing paths

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
        _currentActiveTiles = new HashSet<GridTile> { gridTile };

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
        if (!IsAdjacent(lastTile, gridTile)) return;

        foreach (var kvp in _activeTiles)
        {
            if (kvp.Key != _currentColor && kvp.Value.Contains(gridTile))
            {
                ResetPath(kvp.Key);
                break;
            }
        }

        if (_currentSelection.Contains(gridTile)) return;
        if (gridTile.IsNode && gridTile.Color != _currentColor) return;

        _currentSelection.Add(gridTile);
        _currentActiveTiles.Add(gridTile);
        gridTile.Color = _currentColor;
        
        _pathVisualizer.AddPoint(_currentColor, gridTile.transform.position);
    }

    private bool IsAdjacent(GridTile a, GridTile b)
    {
        return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) + Mathf.Abs(a.Coordinate.y - b.Coordinate.y) == 1;
    }

    private void Backtrack()
    {
        GridTile lastTile = _currentSelection[_currentSelection.Count - 1];
        if (!lastTile.IsNode) lastTile.Color = ColorType.None;

        _currentActiveTiles.Remove(lastTile);
        _currentSelection.RemoveAt(_currentSelection.Count - 1);

        _pathVisualizer.RemoveLastPoint(_currentColor);
    }

    public void ValidatePath()
    {
        if (_currentSelection != null && _currentSelection.Count > 1 &&
            _currentSelection[_currentSelection.Count - 1].IsNode)
        {
            _paths[_currentColor] = new List<GridTile>(_currentSelection);
            _activeTiles[_currentColor] = new HashSet<GridTile>(_currentActiveTiles);

            _undoStack.Push(_currentColor); 

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
        _activeTiles.Remove(color);
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
        _activeTiles.Clear();
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