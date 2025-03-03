using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UserInput : MonoBehaviour
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
            _isDragging = true;
            
        }

        if (Input.GetMouseButton(0) && _isDragging) 
        {
            TrySelectTile();
        }

        if (Input.GetMouseButtonUp(0)) 
        {
            _isDragging = false;
        }
    }

    private void TrySelectTile()
    {
        Vector3 worldPoint = _camera.ScreenToWorldPoint(Input.mousePosition);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPoint);

        if (hitCollider != null)
        {
            IGridTile tile = hitCollider.GetComponent<IGridTile>();
            if (tile != null )
            {
                Debug.Log($"Selected { tile } tile.");
               
            }
        }
    }
}