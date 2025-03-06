using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer _lineRenderer;
    private List<Vector3> _linePositions = new List<Vector3>();

    // private void Awake()
    // {
    //     _lineRenderer = gameObject.AddComponent<LineRenderer>();
    //
    //     _lineRenderer.startWidth = 0.2f;
    //     _lineRenderer.endWidth = 0.2f;
    //     _lineRenderer.numCornerVertices = 100;
    //     _lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    // }

    public void SetColor(ColorType col)
    {
        Color color = ColorGroupSO.Default.GetColor(col);
        _lineRenderer.startColor = color;
        _lineRenderer.endColor = color;
    }

    public void ClearLine()
    {
        _lineRenderer.positionCount = 0;
        _linePositions.Clear();
    }

    public void AddPoint(Vector3 position)
    {
        _linePositions.Add(position);
        _lineRenderer.positionCount = _linePositions.Count;
        _lineRenderer.SetPositions(_linePositions.ToArray());
    }

    public void RemoveLastPoint()
    {
        if (_linePositions.Count > 0)
        {
            _linePositions.RemoveAt(_linePositions.Count - 1);
            _lineRenderer.positionCount = _linePositions.Count;
            _lineRenderer.SetPositions(_linePositions.ToArray());
        }
    }
}
