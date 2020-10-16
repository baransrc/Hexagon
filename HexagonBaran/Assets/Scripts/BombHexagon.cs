using TMPro;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class BombHexagon : Hexagon
{
    [SerializeField] private TextMeshPro counterText;
    
    private SpriteRenderer _spriteRenderer;
    private int _counter;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Explode()
    {
        if (_counter <= 0)
        {
            GameManager.EndGame();
        }
        
        Recycle();
    }

    public override void Initialize(GameManager gameManager, Colors color)
    {
        GameManager = gameManager;

        GameManager.OnTurnEnded += OnTurnEnded;

        _counter = GameManager.GetInitialBombCounterValue();
        
        SetCounterIndicator();
        
        SetColor(color);
    }

    private void SetColor(Colors color)
    {
        _color = color;
        _spriteRenderer.color = GameManager.GetColorRgba(_color);
    }

    private void SetCounterIndicator()
    {
        counterText.text = _counter.ToString();
    }

    private void OnTurnEnded()
    {
        _counter--;
        SetCounterIndicator();

        if (_counter <= 0)
        {
            Explode();
        }
    }

    protected override void Recycle()
    {
        SetColor(Colors.Colorless);
        
        Cell.Hexagon = null;
        Cell = null;
        
        _counter = 0;

        LocalPosition = Pool.SharedInstance.ItemSpawnLocation;

        GameManager.OnTurnEnded -= OnTurnEnded;
        
        GameManager = null;
        gameObject.SetActive(false);
    }
}