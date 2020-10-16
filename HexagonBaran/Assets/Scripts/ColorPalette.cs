using System;
using System.Collections.Generic;
using UnityEngine;

public class ColorPalette : MonoBehaviour
{
    [SerializeField] private List<ColorPair> colorPalette;

    private void Awake()
    {
        CheckForColorPalette();
    }

    private void CheckForColorPalette()
    {
        if (colorPalette == null || colorPalette.Count <= 0)
        {
            throw new InvalidOperationException("ColorPalette cannot be empty or null.");
        }

        foreach (var colorPair in colorPalette)
        {
            if (colorPalette.FindAll(x => x.color == colorPair.color).Count > 1)
            {
                throw new InvalidOperationException("ColorPalette cannot contain same item more than once: (" + colorPair.color + ").");
            }
        }
    }

    public UnityEngine.Color GetColor(Colors color)
    {
        return colorPalette.Find(x => x.color == color).rgba;
    }
}
