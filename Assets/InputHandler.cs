using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    private Camera _camera;
    private bool _isDragging = false;

    private void Awake()
    {
        _camera = Camera.main;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TrySelectTile();
        }

        if (_isDragging && Input.GetMouseButton(0))
        {
            TrySelectTile();
        }

        if (Input.GetMouseButtonUp(0))
        {
            _isDragging = false;
            
            EventManager.DoFireOnEndPath();
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
                if (!_isDragging)
                {
                    EventManager.DoFireOnStartPath(gridTile);
                    _isDragging = true;
                }
                else
                {
                    EventManager.DoFireOnTileAdded(gridTile);
                }
            }
        }
    }
}
