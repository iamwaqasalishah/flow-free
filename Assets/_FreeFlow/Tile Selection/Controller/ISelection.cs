using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISelection 
{
    void Clear();

    bool IsLast(GridTile gameTile);

    bool Contains(GridTile gameTile);

    bool IsEmpty { get; }

    void Add(GridTile gameTile);

    void Remove(GridTile gameTile);

    int Count { get; }

    GridTile Get(int index);

    GridTile GetLast();
}
