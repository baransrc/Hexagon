using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    [FormerlySerializedAs("_selectedBackground")] [SerializeField] private SpriteRenderer selectedBackground;
    public Hexagon Hexagon { get; set; }

    private static int _idCounter = 0;
    private int _id;
    
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

    public int Id
    {
        get
        {
            return _id;
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
        _id = _idCounter;
        _idCounter++;
        
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        selectedBackground.enabled = selected;
        // Debug.Log("Cell (" + LocalPosition + ") backgroundEnabled: " + selectedBackground.enabled);
    }

}