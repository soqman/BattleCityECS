using System;
using UnityEngine;

public class GridSystem<TGridObject>
{
    private int _width;
    private int _height;
    private float _cellSize;
    private TGridObject[,] _gridArray;
    private Vector3 _originPosition;
    [SerializeField] private bool showDebug=true;

    public GridSystem(int width, int height, float cellSize, Vector3 originPosition)
    {
        _width = width;
        _height = height;
        CellSize = cellSize;
        _originPosition = originPosition;
        
        _gridArray=new TGridObject[width,height];
        
        for (var x = 0; x < _gridArray.GetLength(0); x++)
        {
            for (var y = 0; y < _gridArray.GetLength(1); y++)
            {
                _gridArray[x, y] = default;
            }
        }

        if (showDebug)
        {
            for (var x = 0; x < _gridArray.GetLength(0); x++)
            {
                for (var y = 0; y < _gridArray.GetLength(1); y++)
                {
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1),Color.white,100f);
                    Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x+1, y ),Color.white,100f);
                }
            }
            Debug.DrawLine(GetWorldPosition(0, height), GetWorldPosition(width, height ),Color.white,100f);
            Debug.DrawLine(GetWorldPosition(width, 0), GetWorldPosition(width, height ),Color.white,100f);
        }
    }

    public float CellSize
    {
        get => _cellSize;
        set => _cellSize = value;
    }

    public int GetWidth()
    {
        return _width;
    }

    public int GetHeight()
    {
        return _height;
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * CellSize + _originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition-_originPosition).x / CellSize);
        y = Mathf.FloorToInt((worldPosition-_originPosition).y / CellSize);
    }

    public void SetGridObject(int x, int y, TGridObject value)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height) return;
        _gridArray[x, y] = value;
    }

    public void SetGridObject(Vector3 worldPosition, TGridObject value)
    {
        GetXY(worldPosition, out var x, out var y);
        SetGridObject(x,y,value);
    }

    public void GetPareOfGridObjectVertical(Vector3 worldPosition, out TGridObject first, out TGridObject second)
    {
        first = GetGridObject(worldPosition+new Vector3(0,0.01f,0));
        second = GetGridObject(worldPosition+new Vector3(0,-0.01f,0));
    }
    
    public void GetPareOfGridObjectHorizontal(Vector3 worldPosition, out TGridObject first, out TGridObject second)
    {
        first = GetGridObject(worldPosition+new Vector3(0.01f,0,0));
        second = GetGridObject(worldPosition+new Vector3(-0.01f,0,0));
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x < 0 || y < 0 || x >= _width || y >= _height) return default(TGridObject);
        return _gridArray[x, y];
    }

    public TGridObject GetGridObject(Vector3 worldPosition)
    {
        GetXY(worldPosition, out var x,out var y);
        return GetGridObject(x, y);
    }
}
