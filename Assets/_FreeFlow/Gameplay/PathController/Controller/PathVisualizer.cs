using System.Collections;
using System.Collections.Generic;
using RDG;
using UnityEngine;

public class PathVisualizer : MonoBehaviour
{
    private Dictionary<ColorType, PathLineRenderer> _lineRenderers = new Dictionary<ColorType, PathLineRenderer>();

    public void CreateLineRenderer(ColorType color, Vector3 startPosition)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            Destroy(_lineRenderers[color].gameObject);
            _lineRenderers.Remove(color);
        }

        GameObject lineObj = new GameObject("PathLine_" + color);
        PathLineRenderer lineRenderer = lineObj.AddComponent<PathLineRenderer>();
        lineRenderer.SetColor(color);
        lineRenderer.AddPoint(startPosition);
        Vibration.Vibrate(10);
        _lineRenderers[color] = lineRenderer;
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

    public void ClearLine(ColorType color)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            _lineRenderers[color].ClearLine();
        }
    }

    public void RemoveLineRenderer(ColorType color)
    {
        if (_lineRenderers.ContainsKey(color))
        {
            Destroy(_lineRenderers[color].gameObject);
            _lineRenderers.Remove(color);
        }
    }

    public void ClearAllLines()
    {
        foreach (var lineRenderer in _lineRenderers.Values)
        {
            Destroy(lineRenderer.gameObject);
        }

        _lineRenderers.Clear();
    }
}