using System.Collections.Generic;
using UnityEngine;

public class TouchPointManager : MonoBehaviour
{
    [SerializeField] private GameObject touchPointPrefab;
    private Grid _grid;
    private int _gridWidth;
    private int _gridHeight;
    private float _hexagonHeight;
    private float _hexagonWidth;
    private float _hexagonOuterOffset;
    private float _hexagonInnerOffset;

    public List<TouchPoint> CreateTouchPoints(Grid grid, float hexagonOuterOffset, float hexagonInnerOffset, float hexagonHeight, float hexagonWidth)
    {
        _grid = grid;
        _gridWidth = _grid.Width;
        _gridHeight = _grid.Height;
        _hexagonOuterOffset = hexagonOuterOffset;
        _hexagonInnerOffset = hexagonInnerOffset;
        _hexagonHeight = hexagonHeight;
        _hexagonWidth = hexagonWidth;
        
        var touchPoints = SetupTouchPoints();

        return touchPoints;
    }
    
    private List<TouchPoint> SetupTouchPoints()
    {
        var touchPoints = new List<TouchPoint>();
        
        for (var i = 0; i < _gridWidth; i++)
        {
            for (var j = 0; j < _gridHeight; j++)
            {
                var dataList = GetTouchPointData(i, j);
                
                foreach (var data in dataList)
                {
                    var touchPoint = Instantiate(touchPointPrefab).GetComponent<TouchPoint>();
                    touchPoint.Initialize(data);
                    touchPoints.Add(touchPoint);
                }
        
                dataList.Clear();
            }
        }

        return touchPoints;
    }

    private Cell[] GetCellGroupLowerLeft(int x, int y)
    {
        var newX = (x == 0) ? 1 : x;
        
        return new[] { _grid[newX,y], _grid[newX-1,y], _grid[(newX % 2 == 0) ? newX-1 : newX, y+1]};
    }

    private Cell[] GetCellGroupLowerRight(int x, int y)
    {
        var newX = (x == _gridWidth - 1) ? _gridWidth - 2 : x;
        
        return new[] { _grid[newX,y], _grid[newX+1,y], _grid[(newX % 2 == 0) ? newX+1 : newX, y+1]};
    }
    
    private Cell[] GetCellGroupMiddleLeft(int x, int y)
    {
        if (y == _gridHeight - 1)
        {
            return x == 0 ? new[] { _grid[x,y], _grid[x+1,y], _grid[x, y-1]} 
                          : new[] {_grid[x, y], _grid[x - 1, y], _grid[x % 2 == 0 ? x : x-1, y - 1]};
        }

        if (y == 0)
        {
            return x == 0 ? new[] { _grid[x,y], _grid[x+1,y+1], _grid[x+1, y]} 
                : new[] { _grid[x,y], _grid[x-1,y], _grid[(x % 2 == 0) ? x-1 : x, y+1]};
        }
        
        return x == 0 ? new[] { _grid[x,y], _grid[x+1,y+1], _grid[x+1, y]} 
                      : new[] { _grid[x,y], _grid[x-1,y], _grid[x-1, (x % 2 == 0) ? y+1 : y-1]};
    }

    private Cell[] GetCellGroupUpperLeft(int x, int y)
    {
        if (y == _gridHeight - 1)
        {
            return new[] {_grid[x, y], _grid[(x == 0) ? x + 1 : x -1, y], _grid[(x % 2 == 0) ? x : x - 1, y - 1]};
        }
        
        if (x == 0)
        {
            return new[] {_grid[x, y], _grid[x, y + 1], _grid[x + 1, y + 1]};
        }

        return x % 2 == 0 ? new[] {_grid[x, y], _grid[x-1, y + 1], _grid[x, y + 1]} 
                          : new[] {_grid[x, y], _grid[x - 1, y], _grid[x, y + 1]};
    }

    private Cell[] GetCellGroupUpperRight(int x, int y)
    {
        if (x == _gridWidth - 1)
        {
            if (y == _gridHeight - 1)
            {
                return new[] {_grid[x, y], _grid[x - 1, y], _grid[x - 1, y - 1]};
            }
            
            return new[] {_grid[x, y], _grid[x - 1, y], _grid[x, y + 1]};
        }
        
        return x % 2 == 0 ? new[] {_grid[x, y], _grid[x, y - 1], _grid[x + 1, y]} 
                          : new[] {_grid[x, y], _grid[x + 1, y], _grid[x + 1, y - 1]};
    }
    
    private Cell[] GetCellGroupMiddleRight(int x, int y)
    {
        return y == _gridHeight - 1 ? new[] {_grid[x, y], _grid[x - 1, y - 1], _grid[x, y - 1]} 
                                    : new[] {_grid[x, y], _grid[x - 1, y], _grid[x, y + 1]};
    }

    private List<TouchPointData> GetTouchPointData( int x, int y)
    {
        var cellPosition = _grid[x, y].LocalPosition;
        var touchPointData = new List<TouchPointData>();
        var hexagonOuterOffsetHalf = _hexagonOuterOffset * 0.5f;
        
        if (y == 0) 
        {
            var lowerLeftData = new TouchPointData
            {
                LocalPosition = cellPosition - new Vector3((_hexagonInnerOffset + (hexagonOuterOffsetHalf)),
                    (_hexagonHeight * 0.25f) + (hexagonOuterOffsetHalf)),
                Cells = GetCellGroupLowerLeft(x, y)
            };
            touchPointData.Add(lowerLeftData);
        
            var lowerRightData = new TouchPointData
            {
                LocalPosition = cellPosition + new Vector3((_hexagonInnerOffset) + (hexagonOuterOffsetHalf),
                    -(_hexagonHeight * 0.25f) - (hexagonOuterOffsetHalf)),
                Cells = GetCellGroupLowerRight(x, y)
            };
            touchPointData.Add(lowerRightData);
        }
        
        if (x == _gridWidth - 1 ) 
        {
            var middleRightData = new TouchPointData
            {
                LocalPosition = cellPosition + new Vector3((_hexagonWidth * 0.5f) + (hexagonOuterOffsetHalf), 0f),
                Cells = GetCellGroupMiddleRight(x, y)
            };
            touchPointData.Add(middleRightData);
        
            var upperRightData = new TouchPointData
            {
                LocalPosition = cellPosition + new Vector3((_hexagonInnerOffset) + (hexagonOuterOffsetHalf),
                    (_hexagonHeight * 0.25f) + (hexagonOuterOffsetHalf)),
                Cells = GetCellGroupUpperRight(x, y)
            };
            touchPointData.Add(upperRightData);
        }
        
        if (x != _gridWidth - 1 && y == _gridHeight - 1 )
        {
            var upperRightData = new TouchPointData
            {
                LocalPosition = cellPosition + new Vector3((_hexagonInnerOffset) + (hexagonOuterOffsetHalf),
                    (_hexagonHeight * 0.25f) + (hexagonOuterOffsetHalf)),
                Cells = GetCellGroupUpperRight(x, y)
            };
            touchPointData.Add(upperRightData);
        }

        var middleLeftData = new TouchPointData
        {
            LocalPosition = cellPosition + new Vector3(- (_hexagonWidth * 0.5f) - (hexagonOuterOffsetHalf), 0f),
            Cells = GetCellGroupMiddleLeft(x, y)
        };
        touchPointData.Add(middleLeftData);

        var upperLeftData = new TouchPointData
        {
            LocalPosition = cellPosition + new Vector3(- (_hexagonInnerOffset) - (hexagonOuterOffsetHalf), (_hexagonHeight * 0.25f) + (hexagonOuterOffsetHalf)),
            Cells = GetCellGroupUpperLeft(x, y)
        };
        touchPointData.Add(upperLeftData);

        return touchPointData;
    }

}
