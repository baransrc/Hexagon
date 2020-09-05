
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

    public override void Initialize(GameManager gameManager, Color color)
    {
        _gameManager = gameManager;
        
        SetColor(color);
    }

    public void SetColor(Color color)
    {
        _color = color;
        _spriteRenderer.color = _gameManager.GetColorRGBA(_color);
    }

    protected override void Recycle()
    {
        DestroyImmediate(gameObject);
    }
}

