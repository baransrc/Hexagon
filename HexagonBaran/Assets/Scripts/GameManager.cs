using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ColorPalette))]
public class GameManager : MonoBehaviour
{
    [SerializeField] private int _gridWidth;
    [SerializeField] private int _gridHeight;
    [SerializeField] private float _hexagonOuterOffset;
    [SerializeField] private float _hexagonUnitSize;
    [SerializeField] private int _hexagonFallOffset;
    [SerializeField] private GameObject _cellPrefab;
    [SerializeField] private GameObject _colorHexagonPrefab;
    [SerializeField] private List<Color> _allowedColors;

    private float _hexagonHeight;
    private float _hexagonWidth;
    private float _hexagonInnerOffset;
    private ColorPalette _colorPalette;

    public Grid Grid { get; private set; }
 
    private void Awake()
    {
        _colorPalette = GetComponent<ColorPalette>();

        SetupWidthAndHeight();
        SetupGrid();
    }

    public UnityEngine.Color GetColorRGBA(Color color)
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

    private Vector3 GetPositionByIndex(float i, float j)
    {
        return new Vector3(i * (_hexagonInnerOffset * 3f + _hexagonOuterOffset), (j * (_hexagonHeight * 0.5f + _hexagonOuterOffset)) + ((i % 2) * (_hexagonHeight * 0.25f + _hexagonOuterOffset * 0.5f)));
    }
}
