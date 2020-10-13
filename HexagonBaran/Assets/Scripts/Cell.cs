using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Cell : MonoBehaviour
{
    [FormerlySerializedAs("_selectedBackground")] [SerializeField] private SpriteRenderer selectedBackground;
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
        selectedBackground.enabled = selected;
        // Debug.Log("Cell (" + LocalPosition + ") backgroundEnabled: " + selectedBackground.enabled);
    }

}