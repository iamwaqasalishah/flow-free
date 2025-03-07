
using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera _camera;
    private bool _isDragging;
    private Vector3 _lastMousePosition;
    private bool _canInteract = true;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void OnEnable()
    {
        EventManager.OnDisableInteraction += DisableInteraction;
    }

    private void OnDisable()
    {
        EventManager.OnDisableInteraction -= DisableInteraction;
    }

    private void Update()
    {
        if (!_canInteract) return;

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
            EventManager.DoFireOnValidatePath();
        }
    }

    private void TrySelectNode()
    {
        IGridTile gridTile = GetTileUnderMouse();
        if (gridTile != null && gridTile.IsNode)
        {
            _isDragging = true;
            _lastMousePosition = Input.mousePosition;
    
            EventManager.DoFireOnStartNewPath(gridTile);
        }
    }

    private void TrySelectTile()
    {
        IGridTile gridTile = GetTileUnderMouse();
        if (gridTile != null)
        {
            EventManager.DoFireOnHandleTileSelection(gridTile);
        }
    }

    private IGridTile GetTileUnderMouse()
    {
        Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            return hitCollider.GetComponent<IGridTile>(); // ✅ Now using the interface
        }
        return null;
    }

    public void DisableInteraction()
    {
        _canInteract = false;
    }
}
