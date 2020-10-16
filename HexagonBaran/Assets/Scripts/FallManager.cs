using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class FallManager : MonoBehaviour
{
    private GameManager _gameManager;
    private Grid _grid;
    
    [SerializeField] private float _timeToFallOneCell;

    public bool Falling { get; private set; }

    public void Initialize(GameManager gameManager, Grid grid)
    {
        _gameManager = gameManager;
        _grid = _gameManager.Grid;
        Falling = false;
    }
    
    public void Fall()
    {
        Falling = false;

        for (var j = 0; j < _grid.Height; j++)
        {
            for (var i = 0; i < _grid.Width; i++)
            {
                var cell = _grid[i, j];
                var hexagon = cell.Hexagon;

                if (hexagon == null)
                {
                    continue;
                }

                var lowestEmptyCell = GetLowestEmptyCell(i, j);

                cell.Hexagon = null;

                if (hexagon.Cell == null || lowestEmptyCell.Id != hexagon.Cell.Id) // A change occurs.
                {
                    _gameManager.Changed = true;
                }
                
                hexagon.Cell = lowestEmptyCell;
                hexagon.Cell.Hexagon = hexagon;
                
                if (hexagon.Cell.LocalPosition != hexagon.LocalPosition)
                {
                    Falling = true;
                    var deltaY = hexagon.LocalPosition.y - hexagon.Cell.LocalPosition.y;
                    deltaY = deltaY < 0.5f ? deltaY : 0.5f;
                    hexagon.LocalPosition = Vector3.Lerp(hexagon.LocalPosition, hexagon.Cell.LocalPosition, Time.deltaTime /
                        (_timeToFallOneCell * deltaY));
                }
            }
        }
    }

    private Cell GetLowestEmptyCell(int x, int y)
    {
        var i = y - 1;

        while (i >= 0 && _grid[x, i].Empty)
        {
            i--;
        }

        i++;

        return _grid[x, i];
    }
}
