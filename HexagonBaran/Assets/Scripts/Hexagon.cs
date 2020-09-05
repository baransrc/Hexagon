using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Hexagon : MonoBehaviour
{
    protected Color _color;
    protected GameManager _gameManager;

    public Cell Cell{ get; set; }

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

    public abstract void Initialize(GameManager gameManager, Color color);

    public abstract void Explode();

    protected abstract void Recycle();
}
