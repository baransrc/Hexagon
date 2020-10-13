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

    public void GoToCell(Cell newCell)
    {
        StartCoroutine(MoveToPosition(0.2f, newCell.LocalPosition));
        Cell = newCell;
    }

    private IEnumerator MoveToPosition(float duration, Vector2 endPosition)
    {
        var step = 0f;

        while (step < 1f)
        {
            step += Time.deltaTime / duration;
            
            step = (step > 1f) ? 1f : step;

            LocalPosition = Vector2.Lerp(LocalPosition, endPosition, step);
            
            yield return null;
        }
    }

    public abstract void Initialize(GameManager gameManager, Color color);

    public abstract void Explode();

    protected abstract void Recycle();
}
