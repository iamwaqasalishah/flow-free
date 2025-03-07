using System.Collections;
using System.Collections.Generic;
using RDG;
using UnityEngine;
using UnityEngine.Serialization;

public class PathVisualizer : MonoBehaviour
{
    [SerializeField] private PathLineRenderer _lineRendererPrefab; 
    private Dictionary<ColorType, PathLineRenderer> _lineRenderers = new Dictionary<ColorType, PathLineRenderer>();
    private Queue<PathLineRenderer> _lineRendererPool = new Queue<PathLineRenderer>();
  

    public void CreateLineRenderer(ColorType color, Vector3 startPosition)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            RemoveLineRenderer(color);
        }

        PathLineRenderer lineRenderer = GetPooledLineRenderer();
        lineRenderer.SetColor(color);
        lineRenderer.AddPoint(startPosition);
        lineRenderer.gameObject.SetActive(true);
        _lineRenderers[color] = lineRenderer;

        Vibration.Vibrate(10);
    }

    public void AddPoint(ColorType color, Vector3 position)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            Vibration.Vibrate(10);
            _lineRenderers[color].AddPoint(position);
        }
    }

    public void RemoveLastPoint(ColorType color)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            _lineRenderers[color].RemoveLastPoint();
        }
    }

    public void RemoveLineRenderer(ColorType color)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            PathLineRenderer lineRenderer = _lineRenderers[color];
            lineRenderer.ClearLine();
            lineRenderer.gameObject.SetActive(false);
            _lineRendererPool.Enqueue(lineRenderer); 
            _lineRenderers.Remove(color);
        }
    }

    public void ClearAllLines()
    {
        foreach (var lineRenderer in _lineRenderers.Values)
        {
            lineRenderer.ClearLine();
            lineRenderer.gameObject.SetActive(false);
            _lineRendererPool.Enqueue(lineRenderer);
        }

        _lineRenderers.Clear();
    }

    private PathLineRenderer GetPooledLineRenderer()
    {
        if (_lineRendererPool.Count > 0)
        {
            return _lineRendererPool.Dequeue();
        }
        else
        {
            PathLineRenderer lineObj = Instantiate(_lineRendererPrefab);
            return lineObj;
        }
    }
}