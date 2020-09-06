using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _selectedBackground;
    public Hexagon Hexagon { get; set; }
    
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

    public bool Empty
    {
        get
        {
            return Hexagon == null;
        }
    }

    private void Awake()
    {
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        _selectedBackground.enabled = selected;
    }

}