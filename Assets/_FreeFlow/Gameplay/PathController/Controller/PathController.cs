using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathController : MonoBehaviour
{
   private Dictionary<ColorType, List<GridTile>> _paths = new Dictionary<ColorType, List<GridTile>>();
    private Dictionary<ColorType, HashSet<GridTile>> _activeTiles = new Dictionary<ColorType, HashSet<GridTile>>();
    private Dictionary<ColorType, PathLineRenderer> _lineRenderers = new Dictionary<ColorType, PathLineRenderer>();

    private List<GridTile> _currentSelection;
    private HashSet<GridTile> _currentActiveTiles;
    private ColorType _currentColor;
    private int _totalNumberOfPaths;

    private ConnectionController _connectionController;
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
        _connectionController = GetComponent<ConnectionController>();
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
            _connectionController.ClearConnections(_currentColor);
        }

        _currentSelection = new List<GridTile> { gridTile };
        _currentActiveTiles = new HashSet<GridTile> { gridTile };

        gridTile.Color = _currentColor;

        // 🟢 Create a new PathLineRenderer for the color
        CreateLineRenderer(gridTile);
    }

    private void CreateLineRenderer(GridTile startTile)
    {
        if (_lineRenderers.ContainsKey(_currentColor))
        {
            Destroy(_lineRenderers[_currentColor].gameObject);
            _lineRenderers.Remove(_currentColor);
        }

        GameObject lineObj = new GameObject("PathLine_" + _currentColor);
        PathLineRenderer lineRenderer = lineObj.AddComponent<PathLineRenderer>();
        lineRenderer.SetColor(_currentColor);
        lineRenderer.AddPoint(startTile.transform.position);

        _lineRenderers[_currentColor] = lineRenderer;
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
                _connectionController.ClearConnections(kvp.Key);
                break;
            }
        }

        if (_currentSelection.Contains(gridTile)) return;
        if (gridTile.IsNode && gridTile.Color != _currentColor) return;

        _currentSelection.Add(gridTile);
        _currentActiveTiles.Add(gridTile);
        gridTile.Color = _currentColor;

        if (_currentSelection.Count > 1)
        {
            _connectionController.CreateConnection(_currentSelection[_currentSelection.Count - 2], gridTile);
        }

        // 🟢 Update LineRenderer
        _lineRenderers[_currentColor]?.AddPoint(gridTile.transform.position);
    }

    private bool IsAdjacent(GridTile a, GridTile b)
    {
        return Mathf.Abs(a.Coordinate.x - b.Coordinate.x) + Mathf.Abs(a.Coordinate.y - b.Coordinate.y) == 1;
    }

    private void Backtrack()
    {
        GridTile lastTile = _currentSelection[_currentSelection.Count - 1];

        _connectionController.RemoveLastConnection(_currentColor);

        if (!lastTile.IsNode) lastTile.Color = ColorType.None;

        _currentActiveTiles.Remove(lastTile);
        _currentSelection.RemoveAt(_currentSelection.Count - 1);

        // 🟢 Remove last point from LineRenderer
        _lineRenderers[_currentColor]?.RemoveLastPoint();
    }

    public void ValidatePath()
    {
        if (_currentSelection != null && _currentSelection.Count > 1 && _currentSelection[_currentSelection.Count - 1].IsNode)
        {
            _paths[_currentColor] = new List<GridTile>(_currentSelection);
            _activeTiles[_currentColor] = new HashSet<GridTile>(_currentActiveTiles);

            _undoStack.Push(_currentColor); // Store path for undo

            _currentSelection = null;
            CheckLevelComplete();
        }
        else
        {
            ResetPath(_currentColor);
            _connectionController.ClearConnections(_currentColor);
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
        _connectionController.ClearConnections(color);

        // 🟢 Remove LineRenderer for this path
        if (_lineRenderers.ContainsKey(color))
        {
            Destroy(_lineRenderers[color].gameObject);
            _lineRenderers.Remove(color);
        }
    }

    /// <summary>
    /// Clears all paths and connections (Reset Button)
    /// </summary>
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
        _connectionController.ClearAllConnections();

        // 🟢 Clear all LineRenderers
        foreach (var lineRenderer in _lineRenderers.Values)
        {
            Destroy(lineRenderer.gameObject);
        }
        _lineRenderers.Clear();
    }

    /// <summary>
    /// Removes only the last placed path and its connections (Undo Button)
    /// </summary>
    public void UndoLastPath()
    {
        if (_undoStack.Count == 0) return;

        ColorType lastColor = _undoStack.Pop();
        ResetPath(lastColor);
        _connectionController.ClearConnections(lastColor);
    }
}