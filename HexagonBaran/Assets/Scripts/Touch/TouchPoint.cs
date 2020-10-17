using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TouchPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer midPoint;

    private Cell[] _cells;

    public Vector3 LocalPosition
    {
        get
        {
            return transform.localPosition;
        }

        set
        {
            transform.localPosition = value;
        }
    }
    
    public Cell this[int index]
    {
        get
        {
            return _cells[index];
        }

        set
        {
            _cells[index] = value;
        }
    }

    public Cell[] GetCells()
    {
        return _cells;
    }
    
    public void Initialize(TouchPointData touchPointData)
    {
        LocalPosition = touchPointData.LocalPosition;
        
        _cells = touchPointData.Cells;
        
        if (_cells.Length != 3)
        {
            throw new InvalidOperationException("TouchPoint must have 3 Cells.");
        }
        
        CalculateMiddlePointPosition();
    }

    private void CalculateMiddlePointPosition()
    {
        var average = _cells.Aggregate(Vector3.zero, (current, cell) => current + cell.LocalPosition) / 3f;

        midPoint.transform.position = average;
    }
    
    public void DetectTouch(bool detected)
    {
        midPoint.enabled = detected;
        
        SelectCells(detected);
    }

    public bool CellsHaveSameColoredHexagons()
    {
        var color = _cells[0].Hexagon.Color;

        return _cells.All(cell => cell.Hexagon.Color == color);
    }

    private void SelectCells(bool selected)
    {
        foreach (var cell in _cells)
        {
            cell.SetSelected(selected);
        }
    }
}
