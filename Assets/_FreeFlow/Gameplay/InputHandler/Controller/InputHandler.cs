
using System;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera _camera;
    private bool _isDragging;
    private Vector3 _lastMousePosition;

   // private PathController _pathController;
    private bool _canInteract = true;
    private void Awake()
    {
        _camera = Camera.main;
       // _pathController = FindObjectOfType<PathController>();
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
        Vector3 worldPoint = _camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, _camera.nearClipPlane));
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            GridTile gridTile = hitCollider.GetComponent<GridTile>();
            if (gridTile != null && gridTile.IsNode)
            {
                _isDragging = true;
                _lastMousePosition = Input.mousePosition;
    
                EventManager.DoFireOnStartNewPath(gridTile);
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
                EventManager.DoFireOnHandleTileSelection(gridTile);
               
            }
        }
    }
    public void DisableInteraction()
    {
        _canInteract = false;
    }
}
