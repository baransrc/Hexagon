using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    [SerializeField] private List<ColorPair> _colorPalette;

    private void Awake()
    {
        CheckForColorPalette();
    }

    private void CheckForColorPalette()
    {
        if (_colorPalette == null || _colorPalette.Count <= 0)
        {
            throw new InvalidOperationException("ColorPalette cannot be empty or null.");
        }

        foreach (var colorPair in _colorPalette)
        {
            if (_colorPalette.FindAll(x => x.Color == colorPair.Color).Count > 1)
            {
                throw new InvalidOperationException("ColorPalette cannot contain same item more than once: (" + colorPair.Color + ").");
            }
        }
    }

    public UnityEngine.Color GetColor(Color color)
    {
        return _colorPalette.Find(x => x.Color == color).RGBA;
    }
}
