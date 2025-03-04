using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionManager : MonoBehaviour
{
    [SerializeField] private Connection connectionPrefab; 
    private Dictionary<ColorType, List<Connection>> connections = new Dictionary<ColorType, List<Connection>>();
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

        if (!connections.ContainsKey(pathColor))
        {
            connections[pathColor] = new List<Connection>();
        }
       
        connections[pathColor].Add(connection);
    }

    public void ContinuePath(ColorType color, List<GridTile> existingPath)
    {
        if (!connections.ContainsKey(color))
        {
            connections[color] = new List<Connection>();
        }

        for (int i = 0; i < existingPath.Count - 1; i++)
        {
            Connection connection = Instantiate(connectionPrefab, transform);
            connection.SetConnection(existingPath[i], existingPath[i + 1]);
            connections[color].Add(connection);
        }
    }

    public void RemoveLastConnection(ColorType color)
    {
        if (connections.ContainsKey(color) && connections[color].Count > 0)
        {
            int lastIndex = connections[color].Count - 1;
            Connection lastConnection = connections[color][lastIndex];

            connections[color].RemoveAt(lastIndex);
            Destroy(lastConnection.gameObject);
        }
    }

    public void ClearConnections(ColorType color)
    {
        if (connections.ContainsKey(color))
        {
            foreach (var connection in connections[color])
            {
                Destroy(connection.gameObject);
            }
            connections[color].Clear();
        }
    }



}
