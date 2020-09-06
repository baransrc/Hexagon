﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TouchPoint : MonoBehaviour
{
    private Camera _mainCamera;
    private Collider2D _collider2D;
    private bool _detectedTouch;
    private bool _memoryDetectedTouch;
    private Cell[] _cells;

    [SerializeField] private SpriteRenderer midPoint;
    
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
    
    
    private void Awake()
    {
        _mainCamera = Camera.main;
        _collider2D = GetComponent<Collider2D>();
        _detectedTouch = false;
        _memoryDetectedTouch = _detectedTouch;
    }

    public void Initialize(TouchPointData touchPointData)
    {
        transform.localPosition = touchPointData.LocalPosition;
        _cells = touchPointData.Cells;
        
        if (_cells.Length != 3)
        {
            throw new InvalidOperationException("TouchPoint must have 3 Cells.");
        }
        
        // CalculateMiddlePointPosition();
    }

    private void CalculateMiddlePointPosition()
    {
        var average = _cells.Aggregate(Vector3.zero, (current, cell) => current + cell.LocalPosition) / 3f;

        midPoint.transform.position = average;
    }

    public void DetectTouch(Vector3 touchPosition)
    {
        SelectCells(_collider2D.OverlapPoint(touchPosition));
    }

    private void SelectCells(bool selected)
    {
        foreach (var cell in _cells)
        {
            cell.SetSelected(selected);
        }
    }
}
