using UnityEngine;

public class Grid
{
    private int _width;
    private int _height;
    private float _cellSize;
    private Vector3 _originPosition;

    public Grid(int width, int height, float cellSize, Vector3 originPosition)
    {
        _width = width;
        _height = height;
        _cellSize = cellSize;
        _originPosition = originPosition;
        ShowLines();
    }

    private void ShowLines()
    {
        for (var x = 0; x < _width; x++)
        {
            for (var y = 0; y < _height; y++)
            {
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1),Color.white,100f);
                Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x+1, y ),Color.white,100f);
            }
        }
        Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width,_height ),Color.white,100f);
        Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width,_height ),Color.white,100f);
    }

    public Vector3 GetWorldPosition(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _originPosition;
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPosition-_originPosition).x / _cellSize);
        y = Mathf.FloorToInt((worldPosition-_originPosition).y / _cellSize);
    }

    public void GetXYPareByVertical(Vector3 worldPosition, out int xTop, out int yTop, out int xBottom, out int yBottom)
    {
        GetXY(worldPosition+new Vector3(0,0.01f,0),out xTop,out yTop);
        GetXY(worldPosition+new Vector3(0,-0.01f,0),out xBottom,out yBottom);
    }
    
    public void GetXYPareByHorizontal(Vector3 worldPosition, out int xLeft, out int yLeft, out int xRight, out int yRight)
    {
        GetXY(worldPosition+new Vector3(-0.01f,0,0), out xLeft, out yLeft);
        GetXY(worldPosition+new Vector3(0.01f,0,0),  out xRight, out yRight);
    }
}
