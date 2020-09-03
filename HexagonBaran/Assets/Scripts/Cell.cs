using UnityEngine;

public class Cell : MonoBehaviour
{
    public Hexagon Hexagon { get; set; }

    public Vector2 LocalPosition
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

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawCube(transform.position - Vector3.forward, new Vector3(1, 1, 1));
    //}
}