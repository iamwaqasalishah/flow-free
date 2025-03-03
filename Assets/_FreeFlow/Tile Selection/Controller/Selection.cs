using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Selection : ISelection
{
    public List<GridTile> Selected { get; } = new List<GridTile>();

    public bool IsEmpty => Selected.Count == 0;

    public virtual void Clear()
    {
        Selected.Clear();
    }

    public GridTile GetLast()
    {
        if (Count > 0)
        {
            return Selected.Last();
        }
        else
        {
            return null;
        }
    }

    public bool IsLast(GridTile gameTile)
    {
        return Selected.Contains(gameTile) && Selected.Last() == gameTile;
    }

    public virtual void Add(GridTile gameTile)
    {
        if (Contains(gameTile) == false)
        {
            Selected.Add(gameTile);
        }
    }

    public virtual void Remove(GridTile gameTile)
    {
        if (Contains(gameTile))
        {
            Selected.Remove(gameTile);
        }
    }

    public bool Contains(GridTile gameTile)
    {
        return Selected.Contains(gameTile);
    }

    public GridTile Get(int index)
    {
        return Selected[index];
    }

    public int Count => Selected.Count;
}

