using System.Collections;
using System.Collections.Generic;
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
            Debug.Log("Lost Condition!");
        }
        
        Recycle();
    }

    public override void Initialize(GameManager gameManager, Colors color)
    {
        _gameManager = gameManager;

        _gameManager.OnTurnEnded += OnTurnEnded;

        _counter = _gameManager.GetInitialBombCounterValue();
        
        SetCounterIndicator();
        
        SetColor(color);
    }

    public void SetColor(Colors color)
    {
        _color = color;
        _spriteRenderer.color = _gameManager.GetColorRgba(_color);
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

        _gameManager.OnTurnEnded -= OnTurnEnded;
        
        _gameManager = null;
        gameObject.SetActive(false);
    }
}