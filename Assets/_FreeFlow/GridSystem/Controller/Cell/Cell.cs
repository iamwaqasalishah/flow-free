using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class CellData
{
    private bool isOccupied;
    private string cellType; 

    public bool IsOccupied => isOccupied;
    public string CellType => cellType;

    public CellData(bool isOccupied, string cellType)
    {
        this.isOccupied = isOccupied;
        this.cellType = cellType;
    }

    public void SetOccupied(bool state)
    {
        isOccupied = state;
    }

    public void SetCellType(string type)
    {
        cellType = type;
    }
}

[Serializable]
public class CellInfo
{
    private int _row;
    private int _column;
    private int _position;


    public CellInfo(int row, int column, int position)
    {
        _row = row;
        _column = column;
        _position = position;
    }

    public int GetRow()
    {
        return _row;
    }

    public int GetColumn()
    {
        return _column;
    }

    public int GetPosition()
    {
        return _position;
    }
}

public class Cell : MonoBehaviour
{
    private CellData _cellData;
    private CellInfo _cellInfo;
    
    public CellInfo GetCellInfo() => _cellInfo;
    public CellData GetCellData() => _cellData;
    
    public void InitializeCellInfo(int row, int column, int position)
    {
        _cellInfo = new CellInfo(row, column, position);
    }

    public void InitializeCellData()
    {
        _cellData = new CellData(false, "cellType");
    }

}