using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class GridTileSelection : MonoBehaviour
{
    private Camera _camera;
    private bool _isDragging = false;
    private Vector3 _lastMousePosition;

    [ShowInInspector] private Dictionary<ColorType, List<GridTile>> _paths = new Dictionary<ColorType, List<GridTile>>();
    [ShowInInspector] private Dictionary<ColorType, HashSet<GridTile>> _activeTiles = new Dictionary<ColorType, HashSet<GridTile>>();

    private ColorType _currentColor;
    private List<GridTile> _currentSelection;
   [SerializeField] private HashSet<GridTile> _currentActiveTiles;

    [SerializeField] private ConnectionManager connectionManager;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectNode();
        }

        if (_isDragging && Input.GetMouseButton(0))
        {
            if (Vector3.Distance(_lastMousePosition, Input.mousePosition) > 0.1f)
            {
                TrySelectTile();
                _lastMousePosition = Input.mousePosition;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            ValidatePath();
        }
    }

    private void TrySelectNode()
    {
        Vector3 worldPoint =
            _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y,
                _camera.nearClipPlane));
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            GridTile gridTile = hitCollider.GetComponent<GridTile>();
            if (gridTile != null && gridTile.IsNode) 
            {
                _currentColor = gridTile.Color;

                // **RESET PATH IF IT EXISTS**
                if (_paths.ContainsKey(_currentColor))
                {
                    ResetPath(_currentColor);
                    connectionManager.ClearConnections(_currentColor);
                    // Ye pura path reset karega
                }

                // **NEW PATH INITIALIZATION**
                _isDragging = true;
                _currentSelection = new List<GridTile>();
                _currentActiveTiles = new HashSet<GridTile>();

                _currentSelection.Add(gridTile);
                _currentActiveTiles.Add(gridTile);
                gridTile.Color = _currentColor;

                _lastMousePosition = Input.mousePosition;
            }
        }
    }

    private void TrySelectTile()
    {
        Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            GridTile gridTile = hitCollider.GetComponent<GridTile>();
            if (gridTile != null)
            {
                HandleTileSelection(gridTile);
            }
        }
    }

    private void HandleTileSelection(GridTile gridTile)
    {
      if (_currentSelection == null) return;

    GridTile lastTile = _currentSelection[_currentSelection.Count - 1];

    // **Backtracking Check** (Allow going back)
    if (_currentSelection.Count > 1 && _currentSelection[_currentSelection.Count - 2] == gridTile)
    {
        Debug.Log("Backtracking... Removing Last Tile");

        connectionManager.RemoveLastConnection(_currentColor);

        if (!lastTile.IsNode) lastTile.Color = ColorType.None; // Remove color
        _currentActiveTiles.Remove(lastTile);
        _currentSelection.RemoveAt(_currentSelection.Count - 1);

        return;
    }

    // **Final Node Restriction (Block new selections but allow backtracking)**
    if (lastTile.IsNode && _currentSelection.Count > 1)
    {
        Debug.Log("Reached Final Node! No further selection allowed.");
        return; // Stop forward selection but allow backtracking
    }

    // **Ensure Adjacent Selection**
    if (Mathf.Abs(lastTile.Coordinate.x - gridTile.Coordinate.x) + Mathf.Abs(lastTile.Coordinate.y - gridTile.Coordinate.y) != 1)
    {
        return; 
    }

    // **Check if this tile is already part of another path**
    foreach (var kvp in _activeTiles)
    {
        if (kvp.Key != _currentColor && kvp.Value.Contains(gridTile))
        {
            Debug.Log($"Intersection Detected! Clearing old path for {kvp.Key}");

            ResetPath(kvp.Key);  // Remove previous path
            connectionManager.ClearConnections(kvp.Key); // Remove all connections
            break;
        }
    }

    // **Prevent Selecting the Same Tile Again**
    if (_currentSelection.Contains(gridTile)) return;

    // **Block if Wrong Color**
    if (gridTile.IsNode && gridTile.Color != _currentColor) return;

    // **Add Tile to Path**
    _currentSelection.Add(gridTile);
    _currentActiveTiles.Add(gridTile);
    gridTile.Color = _currentColor;

    // **Create Connection**
    if (_currentSelection.Count > 1)
    {
        connectionManager.CreateConnection(_currentSelection[_currentSelection.Count - 2], gridTile);
    }
}

//  **Reset Function for Overlapping Paths**
private void ResetPath(ColorType color)
{
    if (!_paths.ContainsKey(color)) return;

    foreach (var tile in _paths[color])
    {
       if(!tile.IsNode) tile.Color = ColorType.None;
    }

    _paths.Remove(color);
    _activeTiles.Remove(color);

    connectionManager.RemoveLastConnection(color);
}
    

    private void ValidatePath()
    {
        if (_currentSelection != null && _currentSelection.Count > 1 &&
            _currentSelection[_currentSelection.Count - 1].IsNode)
        {
            // Save the path for this color
            _paths[_currentColor] = new List<GridTile>(_currentSelection);
            _activeTiles[_currentColor] = new HashSet<GridTile>(_currentActiveTiles);
            _currentSelection = null; // Lock Path
        }
        else
        {
            ResetPath(_currentColor); // Invalid Path
        }
    }

   
}