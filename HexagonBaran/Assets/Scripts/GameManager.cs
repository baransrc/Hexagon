using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ColorPalette))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private float _hexagonOuterOffset;
    [SerializeField] private float _hexagonUnitSize;
    [SerializeField] private int _hexagonFallOffset;
    [SerializeField] private GameObject _touchPointPrefab;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _colorHexagonPrefab;
    [SerializeField] private List<Color> _allowedColors;
    [SerializeField] private TouchManager _touchManager;

    private float _hexagonHeight;
    private float _hexagonWidth;
    private float _hexagonInnerOffset;
    private ColorPalette _colorPalette;
    
    private List<TouchPoint> _touchPoints;
    private TouchPoint _selectedTouchPoint;
    
    public Grid Grid { get; private set; }
    
    private void Awake()
    {
        _colorPalette = GetComponent<ColorPalette>();
        _touchPoints = new List<TouchPoint>();

        SubscribeToEvents();
        
        SetupWidthAndHeight();
        SetupGrid();
        SetupTouchPoints();
    }

    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SubscribeToEvents()
    {
        _touchManager.OnDraggedRight += TurnClockwise;
        _touchManager.OnDraggedLeft += TurnCounterClockwise;
        _touchManager.OnDraggedUp += TurnCounterClockwise;
        _touchManager.OnDraggedDown += TurnClockwise;
        _touchManager.OnClicked += ProcessTouch;
    }

    private void UnsubscribeFromEvents()
    {
        _touchManager.OnDraggedRight -= TurnClockwise;
        _touchManager.OnDraggedLeft -= TurnCounterClockwise;
        _touchManager.OnDraggedUp -= TurnCounterClockwise;
        _touchManager.OnDraggedDown -= TurnClockwise;
        _touchManager.OnClicked -= ProcessTouch;
    }

    /* Make TurnClockwise and TurnCounterClockWise have their targets turning one turn while there is no match or
       search for matches consisting of their cells, if there is one, turn to that combination, 
       if there is no match in all turns, then turn 360 degrees and stop.
    */
    
    private void TurnClockwise()
    {
        if (_selectedTouchPoint == null)
        {
            return;
        }
        
        var hexagons = new List<Hexagon>();
        var cells = _selectedTouchPoint.GetCells();

        foreach (var cell in cells)
        {
            hexagons.Add(cell.Hexagon);
        }

        for (var i = 0; i < hexagons.Count; i++)
        {
            var current = hexagons[(i + 1) % hexagons.Count];
            
            current.GoToCell(cells[i]);
            
            cells[i].Hexagon = current;
        }
    }

    private void TurnCounterClockwise()
    {
        if (_selectedTouchPoint == null)
        {
            return;
        }
        
        var hexagons = new List<Hexagon>();
        var cells = _selectedTouchPoint.GetCells();
        
        foreach (var cell in cells)
        {
            hexagons.Add(cell.Hexagon);
        }
        
        for (var i = 0; i < hexagons.Count; i++)
        {
            var current = hexagons[(i + hexagons.Count - 1) % hexagons.Count];
            
            current.GoToCell(cells[i]);
            
            cells[i].Hexagon = current;
        }
    }

    private void ProcessTouch(Vector3 position)
    {
        var hit2D = Physics2D.Raycast(position, Vector2.zero);

        if (hit2D.collider == null)
        {
            if (_selectedTouchPoint == null)
            {
                return;
            }
            
            _selectedTouchPoint.DetectTouch(false);
            _selectedTouchPoint = null;
            
            return;
        }

        var touchPointGameObject = hit2D.collider.gameObject;
        
        if (!touchPointGameObject.CompareTag("TouchPoint"))
        {
            return;
        }

        if (_selectedTouchPoint != null)
        {
            _selectedTouchPoint.DetectTouch(false);
        }
        
        var touchPoint = touchPointGameObject.GetComponent<TouchPoint>();

        touchPoint.DetectTouch(true);

        _selectedTouchPoint = touchPoint;
    }
    
    public UnityEngine.Color GetColorRgba(Color color)
    {
        return _colorPalette.GetColor(color);
    }

    private Color GetRandomColor()
    {
        var random = Random.Range(0, _allowedColors.Count);

        return _allowedColors[random];
    }

    private ColorHexagon GetColorHexagon()
    {
        return Instantiate(_colorHexagonPrefab).GetComponent<ColorHexagon>();
    }

    /// <summary>
    /// Assumes width of hexagon as exactly 1 world unit
    /// </summary>
    private void SetupWidthAndHeight() 
    {
        _hexagonWidth = _hexagonUnitSize;
        _hexagonHeight = _hexagonWidth * Mathf.Sqrt(3f);
        _hexagonInnerOffset = _hexagonWidth * 0.25f; 
    }

    private void SetupGrid()
    {
        Grid = new Grid(_gridWidth, _gridHeight);

        var width = (((_gridWidth) - 1f) * _hexagonOuterOffset) + ((_gridWidth - 1) * _hexagonWidth * 0.75f);
        var height = (_hexagonHeight * (_gridHeight + 0.5f)) + (_gridHeight * _hexagonOuterOffset);
        var offset = new Vector3(width * 0.5f, height * 0.25f);

        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                var cell = Instantiate(_cellPrefab, transform).GetComponent<Cell>();

                var position = GetPositionByIndex(i, j);

                cell.LocalPosition = position - offset;

                Grid[i, j] = cell;

                var hexagon = GetColorHexagon();

                hexagon.Initialize(this, GetRandomColor());

                hexagon.LocalPosition = GetPositionByIndex(i, j + _hexagonFallOffset) - offset;

                cell.Hexagon = hexagon;
            }
        }
    }

    private void SetupTouchPoints()
    {
        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                CreateTouchPoint(i,j);
            }
        }
    }

    private Cell[] GetCellGroupLowerLeft(int x, int y)
    {
        var newX = (x == 0) ? 1 : x;
        
        return new Cell [3] { Grid[newX,y], Grid[newX-1,y], Grid[(newX % 2 == 0) ? newX-1 : newX, y+1]};
    }

    private Cell[] GetCellGroupLowerRight(int x, int y)
    {
        var newX = (x == _gridWidth - 1) ? _gridWidth - 2 : x;
        
        return new Cell [3] { Grid[newX,y], Grid[newX+1,y], Grid[(newX % 2 == 0) ? newX+1 : newX, y+1]};
    }

    
    // TODO: Fix this.
    private Cell[] GetCellGroupMiddleLeft(int x, int y)
    {
        if (y == _gridHeight - 1)
        {
            return x == 0 ? new Cell [3] { Grid[x,y], Grid[x+1,y], Grid[x, y-1]} 
                          : new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x % 2 == 0 ? x : x-1, y - 1]};
        }
        return x == 0 ? new Cell [3] { Grid[x,y], Grid[x+1,y+1], Grid[x+1, y]} 
                      : new Cell [3] { Grid[x,y], Grid[x-1,y], Grid[(x % 2 == 0) ? x-1 : x, y+1]};
    }

    private Cell[] GetCellGroupUpperLeft(int x, int y)
    {
        if (y == _gridHeight - 1)
        {
            return new Cell [3] {Grid[x, y], Grid[(x == 0) ? x + 1 : x -1, y], Grid[(x % 2 == 0) ? x : x - 1, y - 1]};
        }
        
        if (x == 0)
        {
            return new Cell [3] {Grid[x, y], Grid[x, y + 1], Grid[x + 1, y + 1]};
        }

        return x % 2 == 0 ? new Cell [3] {Grid[x, y], Grid[x-1, y + 1], Grid[x, y + 1]} 
                          : new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x, y + 1]};
    }

    private Cell[] GetCellGroupUpperRight(int x, int y)
    {
        if (x == _gridWidth - 1)
        {
            if (y == _gridHeight - 1)
            {
                return new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x - 1, y - 1]};
            }
            
            return new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x, y + 1]};
        }
        
        return x % 2 == 0 ? new Cell [3] {Grid[x, y], Grid[x, y - 1], Grid[x + 1, y]} 
                          : new Cell [3] {Grid[x, y], Grid[x + 1, y], Grid[x + 1, y - 1]};
    }
    
    private Cell[] GetCellGroupMiddleRight(int x, int y)
    {
        return y == _gridHeight - 1 ? new Cell [3] {Grid[x, y], Grid[x - 1, y - 1], Grid[x, y - 1]} 
                                    : new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x, y + 1]};
    }
    
    private void CreateTouchPoint(int x, int y)
    {
        var cellPosition = Grid[x, y].LocalPosition;
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

        foreach (var data in touchPointData)
        {
            var touchPoint = Instantiate(_touchPointPrefab).GetComponent<TouchPoint>();
            touchPoint.Initialize(data);
            _touchPoints.Add(touchPoint);
        }
        
        touchPointData.Clear();
    }

    private Vector3 GetPositionByIndex(float i, float j)
    {
        return new Vector3(i * (_hexagonInnerOffset * 3f + _hexagonOuterOffset), (j * (_hexagonHeight * 0.5f + _hexagonOuterOffset)) + (((i + 1) % 2) * (_hexagonHeight * 0.25f + _hexagonOuterOffset * 0.5f)));
    }
}
