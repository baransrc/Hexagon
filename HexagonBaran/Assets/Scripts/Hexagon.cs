using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Hexagon : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;

    public Vector3 Position 
    { 
        get
        {
            return transform.position; 
        } 

        set
        {
            transform.position = value;
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
