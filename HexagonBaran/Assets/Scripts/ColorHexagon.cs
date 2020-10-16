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
        GameManager = gameManager;
        
        SetColor(color);
    }

    private void SetColor(Colors color)
    {
        _color = color;
        _spriteRenderer.color = GameManager.GetColorRgba(_color);
    }

    protected override void Recycle()
    {
        SetColor(Colors.Colorless);
        
        Cell.Hexagon = null;
        Cell = null;

        LocalPosition = Pool.SharedInstance.ItemSpawnLocation;
        
        GameManager = null;
        
        gameObject.SetActive(false);
    }
}

