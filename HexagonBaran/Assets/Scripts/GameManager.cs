using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ColorPalette))]
public class GameManager : MonoBehaviour
{
    [Header("Color Configuration:")]
    [SerializeField] private List<Colors> allowedColors;
    private ColorPalette _colorPalette;
    
    [Header("Grid Configuration:")]
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
                     public Grid Grid { get; private set; }
    
    [Header("Hexagon Configuration:")]
    [SerializeField] private int hexagonBombSpawningFrequency;
    [SerializeField] private float hexagonOuterOffset;
    [SerializeField] private float hexagonUnitSize;
    [SerializeField] private int hexagonFallOffset;
                     private float _hexagonHeight;
                     private float _hexagonWidth;
                     private float _hexagonInnerOffset;
    
    [Header("Prefabs:")]
    [SerializeField] private GameObject cellPrefab;
    
    [Header("Touch:")]
    [SerializeField] private TouchPointManager touchPointManager;
    [SerializeField] private TouchManager touchManager;
                     private Dictionary<int, List<TouchPoint>> _touchPointsByCellId; // This holds touch points that include a certain cell using unique cell Ids.
                     private List<TouchPoint> _touchPoints;
                     private TouchPoint _selectedTouchPoint;
    
    [Header("Score:")]
    [SerializeField] private ScoreManager scoreManager;
                     public int Score { get; private set; }
    
    [Header("Camera:")]
    [SerializeField] private CameraShake cameraShake;
    
    
    // Events:
    public delegate void TurnEndManager();
    public event TurnEndManager OnTurnEnded;

    // Flags:
    private int _movementLock;
    private int _bombSpawningThreshold;
    private bool _turnShouldEnd;
    private bool _gameEnded;
    private bool _paused;
    public bool Paused
    {
        get
        {
            return _paused;
        }

        set
        {
            _paused = value;
            Time.timeScale = _paused ? 0f : 1f;
        }
    }
    public bool Changed { get; set; }
    
    // Mechanics Managers:
    private FallManager _fallManager;

    
    private void Awake()
    {
        _colorPalette = GetComponent<ColorPalette>();
        _fallManager = GetComponent<FallManager>();
        
        _touchPoints = new List<TouchPoint>();
        _touchPointsByCellId = new Dictionary<int, List<TouchPoint>>();
        
        _movementLock = 0;
        
        _gameEnded = false;
        
        _bombSpawningThreshold = hexagonBombSpawningFrequency;
        
        SubscribeToEvents();
    }

    private void Start()
    {
        SetupWidthAndHeight();
        SetupGrid();
        
        SetupTouchPoints();
        
        scoreManager.DisplayScore(Score);

        _fallManager.Initialize(this, Grid);
    }
    
    private void OnDestroy()
    {
        UnsubscribeFromEvents();
    }

    private void SaveScore()
    {
        PlayerPrefs.SetInt(StoredVariables.Lastscore, Score);
    }

    public void EndGame()
    {
        _gameEnded = true;
        Paused = false;
        
        SaveScore();
        
        SceneManager.LoadScene("GameOverScene");
    }

    private void SubscribeToEvents()
    {
        touchManager.OnDraggedRight += TurnClockwise;
        touchManager.OnDraggedLeft += TurnCounterClockwise;
        touchManager.OnDraggedUp += TurnClockwise;
        touchManager.OnDraggedDown += TurnCounterClockwise;
        touchManager.OnClicked += ProcessTouch;
    }

    private void UnsubscribeFromEvents()
    {
        touchManager.OnDraggedRight -= TurnClockwise;
        touchManager.OnDraggedLeft -= TurnCounterClockwise;
        touchManager.OnDraggedUp -= TurnClockwise;
        touchManager.OnDraggedDown -= TurnCounterClockwise;
        touchManager.OnClicked -= ProcessTouch;
    }

    private bool LookForMatches()
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
            var hexagon = pair.Value.Hexagon;
            var particle = Pool.SharedInstance.GetPooledObject(PoolingId.ExplosionParticle).GetComponent<ExplosionParticle>();
            var color = GetColorRgba(hexagon.Color);
            
            particle.Play(color, hexagon.LocalPosition);
            
            hexagon.Explode();
        }

        if (scoreToAdd == 0)
        {
            return false;
        }
        
        cameraShake.TriggerShake();
        
        Score += scoreToAdd;
        
        Pool.SharedInstance.GetPooledObject(PoolingId.ScoreObject).GetComponent<ScoreObject>().ShowScore(scoreToAdd);

        return true;
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
        if (Paused)
        {
            return;
        }

        if (_fallManager.Falling)
        {
            return;
        }
        
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
                    break;
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
        if (Paused)
        {
            return;
        }

        if (_fallManager.Falling)
        {
            return;
        }

        if (_movementLock > 0)
        {
            return;
        }
        
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

        AudioManager.Instance.PlaySound(Sounds.Click);
        
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
        var random = Random.Range(0, allowedColors.Count);

        return allowedColors[random];
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
        _hexagonWidth = hexagonUnitSize;
        _hexagonHeight = _hexagonWidth * Mathf.Sqrt(3f);
        _hexagonInnerOffset = _hexagonWidth * 0.25f; 
    }

    private void SetupGrid()
    {
        Grid = new Grid(gridWidth, gridHeight);

        var width = (((gridWidth) - 1f) * hexagonOuterOffset) + ((gridWidth - 1) * _hexagonWidth * 0.75f);
        var height = (_hexagonHeight * (gridHeight + 0.5f)) + (gridHeight * hexagonOuterOffset);
        var offset = new Vector3(width * 0.5f, height * 0.25f);

        for (var i = 0; i < gridWidth; i++)
        {
            for (var j = 0; j < gridHeight; j++)
            {
                var cell = Instantiate(cellPrefab, transform).GetComponent<Cell>();

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
            _bombSpawningThreshold += hexagonBombSpawningFrequency;

            return GetBombHexagon();
        }

        return GetColorHexagon();
    }

    private void Fill()
    {
        var width = (((gridWidth) - 1f) * hexagonOuterOffset) + ((gridWidth - 1) * _hexagonWidth * 0.75f);
        var height = (_hexagonHeight * (gridHeight + 0.5f)) + (gridHeight * hexagonOuterOffset);
        var offset = new Vector3(width * 0.5f, height * 0.25f);

        for (var i = 0; i < gridWidth; i++)
        {
            for (var j = 0; j < gridHeight; j++)
            {
                var cell = Grid[i, j];

                if (!cell.Empty)
                {
                    continue;
                }

                Changed = true;
                
                var hexagon = GetHexagonByScore();

                hexagon.Initialize(this, GetRandomColor()); // Use (Colors)((j + i)%7) as Colors argument to check for lose condition with no movements left.

                hexagon.LocalPosition = GetPositionByIndex(i, j + hexagonFallOffset) - offset;

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
        _touchPoints = touchPointManager.CreateTouchPoints(Grid, hexagonOuterOffset, _hexagonInnerOffset,
            _hexagonHeight, _hexagonWidth);
        
        
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
        return new Vector3(i * (_hexagonInnerOffset * 3f + hexagonOuterOffset), (j * (_hexagonHeight * 0.5f + hexagonOuterOffset)) + (((i + 1) % 2) * (_hexagonHeight * 0.25f + hexagonOuterOffset * 0.5f)));
    }
    
    private bool DetermineIfThereAreMovesLeft()
    {
        var foundPossibleMatch = false;
        
        foreach (var touchPoint in _touchPoints)
        {
            var cells = touchPoint.GetCells();
            
            var initialHexagons = new List<Hexagon>();

            for (var j = 0; j < cells.Length; j++)
            {
                var hexagons = new List<Hexagon>();
            
                foreach (var cell in cells)
                {
                    hexagons.Add(cell.Hexagon);

                    if (j != 0)
                    {
                        continue;
                    }
                    
                    initialHexagons.Add(cell.Hexagon);
                }

                for (var i = 0; i < hexagons.Count; i++)
                {
                    var next = (i + 1) % hexagons.Count;
                    var current = hexagons[next];

                    cells[i].Hexagon = current;
                }

                var stopTurning = false;
            
                foreach (var cell in cells)
                {
                    var touchPointsContainingCell = _touchPointsByCellId[cell.Id];
                    if (touchPointsContainingCell.Exists(x => x.CellsHaveSameColoredHexagons()))
                    {
                        stopTurning = true;
                        foundPossibleMatch = true;
                        break;
                    }
                }

                if (stopTurning)
                {
                    break;
                }
            }

            for (var index = 0; index < cells.Length; index++)
            {
                var cell = cells[index];
                cell.Hexagon = initialHexagons[index];
            }

            if (foundPossibleMatch)
            {
                break;
            }
        }

        return foundPossibleMatch;
    }
    
    private void Match()
    {
        if (_fallManager.Falling || _movementLock > 0 || !Changed)
        {
            return;
        }
        
        var foundMatches = LookForMatches();
        scoreManager.DisplayScore(Score);

        if (!foundMatches)
        {
            var movesLeft = DetermineIfThereAreMovesLeft();

            if (!movesLeft)
            {
                EndGame();
            }
        }
            
        Changed = false;
    }

    private void Fall()
    {
        if (_movementLock > 0)
        {
            return;
        }
        
        _fallManager.Fall();
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
        if (_gameEnded)
        {
            return;
        }
        
        Fall();
        
        Fill();
        
        Match();
        
        EndTurn();
    }
}
