using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GameManager))]
public class FallManager : MonoBehaviour
{
    private GameManager _gameManager;
    private Grid _grid;

    [SerializeField] private float _timeToFallOneCell;

    private void Awake()
    {
        _gameManager = GetComponent<GameManager>();
        _grid = _gameManager.Grid;
    }

    private void Fall()
    {
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
                hexagon.Cell = lowestEmptyCell;
                hexagon.Cell.Hexagon = hexagon;
                
                if (hexagon.Cell.LocalPosition != hexagon.LocalPosition)
                {
                    hexagon.LocalPosition = Vector3.Lerp(hexagon.LocalPosition, hexagon.Cell.LocalPosition, Time.deltaTime / _timeToFallOneCell);
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

    private void Update()
    {
        Fall();
    }
}
