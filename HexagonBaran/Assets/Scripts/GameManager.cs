using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float _gridWidth;
    [SerializeField] private float _gridHeight;
    [SerializeField] private float _hexagonOuterOffset;
    [SerializeField] private float _hexagonUnitSize;
    [SerializeField] private GameObject _cellPrefab;


    private float _hexagonHeight;
    private float _hexagonWidth;
    private float _hexagonInnerOffset;

    private void Awake()
    {
        SetupWidthAndHeight();
        SetupGrid();
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
            }
        }
    }

    private Vector3 GetPositionByIndex(float i, float j)
    {
        return new Vector3(i * (_hexagonInnerOffset * 3f + _hexagonOuterOffset), (j * (_hexagonHeight * 0.5f + _hexagonOuterOffset)) + ((i % 2) * (_hexagonHeight * 0.25f + _hexagonOuterOffset * 0.5f)));
    }
}
