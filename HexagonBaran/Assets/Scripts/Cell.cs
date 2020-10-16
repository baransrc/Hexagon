using UnityEngine;

public class Cell : MonoBehaviour
{
    [SerializeField] private SpriteRenderer selectedBackground;
    
    private static int _idCounter = 0;

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
    public int Id { get; private set; }
    public bool Empty
    {
        get
        {
            return Hexagon == null;
        }
    }

    private void Awake()
    {
        Id = _idCounter;
        _idCounter++;
        
        SetSelected(false);
    }

    public void SetSelected(bool selected)
    {
        selectedBackground.enabled = selected;
    }
}