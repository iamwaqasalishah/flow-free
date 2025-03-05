using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionController : MonoBehaviour
{
    [SerializeField] private Connection connectionPrefab; 
    private Dictionary<ColorType, List<Connection>> _connections = new Dictionary<ColorType, List<Connection>>();
    private Dictionary<ColorType, GridTile> tempLastTile = new Dictionary<ColorType, GridTile>();

// Last tile ko temporarily store karne ka method
    public void SetTemporaryLastTile(ColorType color, GridTile lastTile)
    {
        tempLastTile[color] = lastTile;
    }

// Last stored tile ko retrieve karne ka method
    public GridTile GetTemporaryLastTile(ColorType color)
    {
        return tempLastTile.ContainsKey(color) ? tempLastTile[color] : null;
    }

    public void CreateConnection(GridTile fromTile, GridTile toTile)
    {
        if (fromTile == null || toTile == null) return;

        Connection connection = Instantiate(connectionPrefab, transform);
        connection.SetConnection(fromTile, toTile);
        ColorType pathColor = fromTile.Color;

        if (!_connections.ContainsKey(pathColor))
        {
            _connections[pathColor] = new List<Connection>();
        }
       
        _connections[pathColor].Add(connection);
    }

    public void ContinuePath(ColorType color, List<GridTile> existingPath)
    {
        if (!_connections.ContainsKey(color))
        {
            _connections[color] = new List<Connection>();
        }

        for (int i = 0; i < existingPath.Count - 1; i++)
        {
            Connection connection = Instantiate(connectionPrefab, transform);
            connection.SetConnection(existingPath[i], existingPath[i + 1]);
            _connections[color].Add(connection);
        }
    }
    public void ClearAllConnections()
    {
        foreach (var connection in _connections.Values)
        {
            foreach (var line in connection)
            {
                Destroy(line.gameObject); // Destroy all line renderers or connection objects
            }
        }
    
        _connections.Clear(); // Clear the dictionary
    }
    public void RemoveLastConnection(ColorType color)
    {
        if (_connections.ContainsKey(color) && _connections[color].Count > 0)
        {
            int lastIndex = _connections[color].Count - 1;
            Connection lastConnection = _connections[color][lastIndex];

            _connections[color].RemoveAt(lastIndex);
            Destroy(lastConnection.gameObject);
        }
    }

    public void ClearConnections(ColorType color)
    {
        if (_connections.ContainsKey(color))
        {
            foreach (var connection in _connections[color])
            {
                Destroy(connection.gameObject);
            }
            _connections[color].Clear();
        }
    }



}
