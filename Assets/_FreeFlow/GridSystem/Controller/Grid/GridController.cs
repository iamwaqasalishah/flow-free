
using UnityEngine;


public class GridController : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private Cell _cellPrefab; 
    [SerializeField] private int _rows = 5;
    [SerializeField] private int _columns = 5;
    [SerializeField] private float _cellSpacing = 1.1f; 

    private Cell[,] _gridCells; 
//hww
    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        _gridCells = new Cell[_rows, _columns];

        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Vector3 position=GetCellPosition(row, col) - GetGridOffset();
                Cell cell = Instantiate(_cellPrefab, position, Quaternion.identity, transform);

                cell.InitializeCellInfo(row, col, row * _columns + col); 
                cell.InitializeCellData(); 

                _gridCells[row, col] = cell;
            }
        }
    }
    private Vector3 GetGridOffset()
    {
        float gridWidth = (_columns-1) * _cellSpacing;
        float gridHeight = (_rows-1) * _cellSpacing;
        
       return new Vector3(gridWidth / 2f, gridHeight / 2f, 0);
    }
   
    private Vector3 GetCellPosition(int row, int column)
    {
        return new Vector3(column * _cellSpacing, row * _cellSpacing, 0);
    }

    public Cell GetCell(int row, int col)
    {
        if (row >= 0 && row < _rows && col >= 0 && col < _columns)
            return _gridCells[row, col];

        return null; 
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;
        for (int row = 0; row < _rows; row++)
        {
            for (int col = 0; col < _columns; col++)
            {
                Vector3 position=GetCellPosition(row, col) - GetGridOffset();
                Gizmos.DrawWireCube(position, Vector3.one * 1f);
            }
        }
    }
}