using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridTile : MonoBehaviour,IGridTile
{
    [SerializeField] private SpriteRenderer _pathEnd;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private Collider2D _inputCollider;
    public List<IGridTile> Neighbors { get; set; } = new List<IGridTile>();
    
    private float _size;
    private bool _isNode;
    private ColorType _color;
    
    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            transform.localScale = Vector3.one * value;
        }
    }
    public ColorType Color
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
    public void SetNeighbors(List<IGridTile> neighbors)
    {
        Neighbors = neighbors;
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

   

    public void Highlight()
    {
        _renderer.color = ColorGroupSO.Default.GetColor(_color);
    }

    public void UnHighlight()
    {
        _renderer.color = ColorGroupSO.Default.GetColor(ColorType.None);
    }
}

