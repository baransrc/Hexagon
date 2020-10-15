using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
    [SerializeField] private List<Colors> _allowedColors;
    [SerializeField] private TouchManager _touchManager;
    [SerializeField] private ScoreManager scoreManager;
    
    public delegate void TurnEndManager();
    public event TurnEndManager OnTurnEnded;
        

    private float _hexagonHeight;
    private float _hexagonWidth;
    private float _hexagonInnerOffset;
    private ColorPalette _colorPalette;

    private Dictionary<int, List<TouchPoint>> _touchPointsByCellId; // This holds touch points that include a certain cell using unique cell Ids.
    private List<TouchPoint> _touchPoints;
    private TouchPoint _selectedTouchPoint;
    private FallManager _fallManager;

    private int _movementLock;
    
    [SerializeField] private int bombSpawningFrequency;
    private int _bombSpawningThreshold;
    private bool _turnShouldEnd;

    public int Score { get; private set; }
    
    public Grid Grid { get; private set; }
    public bool Changed { get; set; }

    private void Awake()
    {
        _colorPalette = GetComponent<ColorPalette>();
        _touchPoints = new List<TouchPoint>();
        _touchPointsByCellId = new Dictionary<int, List<TouchPoint>>();
        _fallManager = GetComponent<FallManager>();
        _movementLock = 0;
        _bombSpawningThreshold = bombSpawningFrequency;
        
        SubscribeToEvents();
    }

    private void Start()
    {
        SetupWidthAndHeight();
        SetupGrid();
        SetupTouchPoints();
        
        scoreManager.DisplayScore(Score);
        
        PopulateTouchPointsByCellId();
        
        _fallManager.Initialize(this, Grid);
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

    private void LookForMatches()
    {
        var cellsToExplode = new Dictionary<int, Cell>();
        
        foreach (var touchPoint in _touchPoints)
        {
            if (!touchPoint.CellsHaveSameColoredHexagons())
            {
                continue;
            }

            var cells = touchPoint.GetCells();

            foreach (var cell in cells)
            {
                if (cellsToExplode.ContainsKey(cell.Id))
                {
                    continue;
                }
                
                cellsToExplode.Add(cell.Id, cell);
            }
        }

        var scoreToAdd = cellsToExplode.Count * 5;

        foreach (var pair in cellsToExplode)
        {
            pair.Value.Hexagon.Explode();
        }

        if (scoreToAdd == 0)
        {
            return;
        }
        
        Score += scoreToAdd;
        
        Pool.SharedInstance.GetPooledObject(PoolingId.ScoreObject).GetComponent<ScoreObject>().ShowScore(scoreToAdd);
    }

    public int GetInitialBombCounterValue()
    {
        return 7;
    }

    public void LockMovement()
    {
        _movementLock++;
    }

    public void UnlockMovement()
    {
        _movementLock--;
        _movementLock = _movementLock < 0 ? 0 : _movementLock;
    }
    
    private void Turn(bool isClockwise)
    {
        if (_movementLock > 0)
        {
            return;
        }
        
        if (_selectedTouchPoint == null)
        {
            return;
        }
       
        var cells = _selectedTouchPoint.GetCells();

        var turnCount = 0;
        
        while (turnCount < cells.Length)
        {
            var hexagons = new List<Hexagon>();
            
            foreach (var cell in cells)
            {
                hexagons.Add(cell.Hexagon);
            }

            for (var i = 0; i < hexagons.Count; i++)
            {
                var next = isClockwise ? (i + 1) % hexagons.Count : (i + hexagons.Count - 1) % hexagons.Count;
                var current = hexagons[next];
                
                current.AddToTurnDestination(cells[i]);
                
                cells[i].Hexagon = current;
            }

            var stopTurning = false;
            
            foreach (var cell in cells)
            {
                var touchPointsContainingCell = _touchPointsByCellId[cell.Id];
                if (touchPointsContainingCell.Exists(touchPoint => touchPoint.CellsHaveSameColoredHexagons()))
                {
                    stopTurning = true;
                    continue;
                }
            }

            if (stopTurning)
            {
                break;
            }

            turnCount++;
        }

        foreach (var cell in cells)
        {
            cell.Hexagon.ExecuteTurns();
        }
        
        if (turnCount == cells.Length)
        {
            return;
        }

        _turnShouldEnd = true;
        Changed = true;
    }
    
    private void TurnClockwise()
    {
        Turn(true);
    }

    private void TurnCounterClockwise()
    {
        Turn(false);
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
    
    public Color GetColorRgba(Colors color)
    {
        return _colorPalette.GetColor(color);
    }

    private Colors GetRandomColor()
    {
        var random = Random.Range(0, _allowedColors.Count);

        return _allowedColors[random];
    }

    private ColorHexagon GetColorHexagon()
    {
        return Pool.SharedInstance.GetPooledObject(PoolingId.ColorHexagon).GetComponent<ColorHexagon>();
    }
    
    private BombHexagon GetBombHexagon()
    {
        return Pool.SharedInstance.GetPooledObject(PoolingId.BombHexagon).GetComponent<BombHexagon>();
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
            }
        }
        
        Fill();
    }

    private Hexagon GetHexagonByScore()
    {
        if (Score >= _bombSpawningThreshold)
        {
            _bombSpawningThreshold += bombSpawningFrequency;

            return GetBombHexagon();
        }

        return GetColorHexagon();
    }

    private void Fill()
    {
        var width = (((_gridWidth) - 1f) * _hexagonOuterOffset) + ((_gridWidth - 1) * _hexagonWidth * 0.75f);
        var height = (_hexagonHeight * (_gridHeight + 0.5f)) + (_gridHeight * _hexagonOuterOffset);
        var offset = new Vector3(width * 0.5f, height * 0.25f);

        for (int i = 0; i < _gridWidth; i++)
        {
            for (int j = 0; j < _gridHeight; j++)
            {
                var cell = Grid[i, j];

                if (!cell.Empty)
                {
                    continue;
                }

                Changed = true;
                
                var hexagon = GetHexagonByScore();

                hexagon.Initialize(this, GetRandomColor());

                hexagon.LocalPosition = GetPositionByIndex(i, j + _hexagonFallOffset) - offset;

                cell.Hexagon = hexagon;
                hexagon.Cell = cell;
            }
        }
    }

    public void ReleaseSelectedTouchPoint()
    {
        if (_selectedTouchPoint == null)
        {
            return;
        }
        
        _selectedTouchPoint.DetectTouch(false);
        _selectedTouchPoint = null;
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
    
    private Cell[] GetCellGroupMiddleLeft(int x, int y)
    {
        if (y == _gridHeight - 1)
        {
            return x == 0 ? new Cell [3] { Grid[x,y], Grid[x+1,y], Grid[x, y-1]} 
                          : new Cell [3] {Grid[x, y], Grid[x - 1, y], Grid[x % 2 == 0 ? x : x-1, y - 1]};
        }

        if (y == 0)
        {
            return x == 0 ? new Cell [3] { Grid[x,y], Grid[x+1,y+1], Grid[x+1, y]} 
                : new Cell [3] { Grid[x,y], Grid[x-1,y], Grid[(x % 2 == 0) ? x-1 : x, y+1]};
        }
        
        return x == 0 ? new Cell [3] { Grid[x,y], Grid[x+1,y+1], Grid[x+1, y]} 
                      : new Cell [3] { Grid[x,y], Grid[x-1,y], Grid[x-1, (x % 2 == 0) ? y+1 : y-1]};
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

    private void PopulateTouchPointsByCellId()
    {
        foreach (var touchPoint in _touchPoints)
        {
            var cells = touchPoint.GetCells();
            
            foreach (var cell in cells)
            {
                if (!_touchPointsByCellId.ContainsKey(cell.Id))
                {
                    _touchPointsByCellId.Add(cell.Id, new List<TouchPoint>());
                }
                
                _touchPointsByCellId[cell.Id].Add(touchPoint);
            }
        }
    }
    
    private Vector3 GetPositionByIndex(float i, float j)
    {
        return new Vector3(i * (_hexagonInnerOffset * 3f + _hexagonOuterOffset), (j * (_hexagonHeight * 0.5f + _hexagonOuterOffset)) + (((i + 1) % 2) * (_hexagonHeight * 0.25f + _hexagonOuterOffset * 0.5f)));
    }
    
    private void Match()
    {
        if (!_fallManager.Falling && _movementLock < 1 && Changed)
        {
            LookForMatches(); // There is an unnecessary execution of this because of one of the booleans.
            scoreManager.DisplayScore(Score);
            Changed = false;
        }
    }

    private void EndTurn()
    {
        if (!_turnShouldEnd || _movementLock > 0)
        {
            return;
        }

        _turnShouldEnd = false;
        OnTurnEnded?.Invoke();
    }
    
    private void Update()
    {
        _fallManager.Fall();

        Fill();
        
        Match();
        
        EndTurn();
    }
}
