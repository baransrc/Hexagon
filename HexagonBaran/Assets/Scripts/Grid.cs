using System;

public class Grid
{
    private Cell[] _cells;
    private int _height;
    private int _width;

    public int Width 
    { 
        get
        {
            return _width;
        }
    }

    public int Height
    {
        get
        {
            return _height;
        }
    }

    public Cell this[int x, int y]
    {
        get
        {
            if (x >= _width || y >= _height)
                throw new InvalidOperationException("There is no cell with such x: " +
                                                    x + " and y: " + y + " in grid.");

            return _cells[_height * x + y];
        }

        set
        {
            _cells[_height * x + y] = value;
        }
    }

    public Grid(int width, int height)
    {
        _cells = new Cell[width * height];
        _height = height;
        _width = width;
    }
}