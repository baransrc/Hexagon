
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ColorHexagon : Hexagon
{
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Explode()
    {
        Recycle();
    }

    public override void Initialize(GameManager gameManager, Colors color)
    {
        _gameManager = gameManager;
        
        SetColor(color);
    }

    public void SetColor(Colors color)
    {
        _color = color;
        _spriteRenderer.color = _gameManager.GetColorRgba(_color);
    }

    protected override void Recycle()
    {
        SetColor(Colors.Colorless);
        
        Cell.Hexagon = null;
        Cell = null;

        LocalPosition = Pool.SharedInstance.ItemSpawnLocation;
        
        _gameManager = null;
        gameObject.SetActive(false);
    }
}

