using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Hexagon : MonoBehaviour
{
    protected Colors _color;
    protected GameManager _gameManager;

    public Cell Cell{ get; set; }

    private Queue<Cell> _turnQueue;

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

    public Colors Color
    {
        get
        {
            return _color;
        }
    }        
    
    public void AddToTurnDestination(Cell newCell)
    {
        if (_turnQueue == null)
        {
            _turnQueue = new Queue<Cell>();
        }
        
        _turnQueue.Enqueue(newCell);
    }

    public void ExecuteTurns()
    {
        StartCoroutine(ExecuteTurnsCoroutine(0.2f, true));
    }
    
    private IEnumerator ExecuteTurnsCoroutine(float durationOneCell, bool lockBoard = false)
    {
        if (lockBoard)
        {
            _gameManager.LockMovement();    
        }

        while (_turnQueue.Count > 0)
        {
            var step = 0f;
            
            Cell = _turnQueue.Dequeue();
            
            var endPosition = Cell.LocalPosition;

            while (step < 1f)
            {
                step += Time.deltaTime / durationOneCell;
            
                step = (step > 1f) ? 1f : step;

                LocalPosition = Vector2.Lerp(LocalPosition, endPosition, step);
            
                yield return null;
            }
        }
        
        if (lockBoard)
        {
            _gameManager.UnlockMovement();    
        }
        
        _gameManager.ReleaseSelectedTouchPoint();
    }
    

    public abstract void Initialize(GameManager gameManager, Colors color);

    public abstract void Explode();

    protected abstract void Recycle();
}
