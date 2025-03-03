// /*
// Created by Darsan
// */

using System.Linq;
using UnityEngine;

public class GridTile : MonoBehaviour,IGridTile
{
    [SerializeField] private SpriteRenderer _pathEnd;
    [SerializeField] private Collider2D _inputCollider;
    
    private float _size;
    private bool _isNode;
    private int _color;
    
    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            transform.localScale = Vector3.one * value;
        }
    }

    
    public int Color
    {
        get => _color;
        set
        {
            _pathEnd.color = ColorGroupSO.Default.GetColor(value);
            _color = value;
        }
    }
    public bool IsNode
    {
        get => _isNode;
        set
        {
            _pathEnd.gameObject.SetActive(value);
            _isNode = value;
        }
    }

    public Vector2Int Coordinate { get; set; }

    public void EnableCollider()
    {
        _inputCollider.enabled = true;
    }

    public void DisableCollider()
    {
        _inputCollider.enabled = false;
    }

   

    public void SelectTile()
    {
        
    }

    public void DeselectTile()
    {
        
    }
}

