using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private Camera _mainCamera;

    public delegate void DragLeftHandler();
    public delegate void DragRightHandler();
    public delegate void DragUpHandler();
    public delegate void DragDownHandler();
    public delegate void ClickHandler(Vector3 position);

    public event DragLeftHandler OnDraggedLeft;
    public event DragRightHandler OnDraggedRight;
    public event DragUpHandler OnDraggedUp;
    public event DragDownHandler OnDraggedDown;
    public event ClickHandler OnClicked;

    private Vector2 _touchStartPos = Vector2.zero;
    private Vector2 _touchEndPos = Vector2.zero;
    private bool _touchIsRegistered = false;

    private void Awake()
    {
        _mainCamera = Camera.main;
        
        Input.multiTouchEnabled = false; 
    }

    private void DetectTouch()
    {
#if UNITY_EDITOR
        
        var mousePosition = _mainCamera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetMouseButtonDown(0))
        {
            _touchStartPos = mousePosition;
        }

        else if (Input.GetMouseButtonUp(0))
        {
            _touchIsRegistered = true;
            
            _touchEndPos = mousePosition;
        }

        else if (Input.GetMouseButton(0))
        {
            // Do Nothing.
        }

        else
        {
            _touchIsRegistered = false;
        }

#elif UNITY_ANDROID || UNITY_IOS

        if (Input.touchCount > 0)
        {
            var touch = Input.GetTouch(0);
            var touchPosition = _mainCamera.ScreenToWorldPoint(touch.position);

            if (touch.phase == TouchPhase.Began)
            {
                _touchStartPos = touchPosition;
                
                return;
            }

            if (touch.phase == TouchPhase.Ended)
            {
                _touchIsRegistered = true;
                
                _touchEndPos = touchPosition;
                
                return;
            }
        }
        
        _touchIsRegistered = false;
#endif
    }

    private void ProcessTouch()
    {
        if (!_touchIsRegistered)
            return;

        var subs = new Vector2((int)_touchEndPos.x - (int)_touchStartPos.x, 
                                       (int)_touchEndPos.y - (int)_touchStartPos.y);

        var direction = subs.normalized;

        if (direction == Vector2.zero) // This is just a click or touch.
        {
            OnClicked?.Invoke(_touchEndPos);
            
            return;
        }

        if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
        {
            if (direction.x > 0)
            {
                OnDraggedRight?.Invoke(); // Debug.Log("Direction is right."); 
            }
            
            else
            {
                OnDraggedLeft?.Invoke(); // Debug.Log("Direction is left.");
            }        
        }

        else 
        {
            if (direction.y > 0)
            {
                OnDraggedUp?.Invoke(); // Debug.Log("Direction is up.");
            }
            
            else
            {
                OnDraggedDown?.Invoke(); // Debug.Log("Direction is down.");
            }
        }
    }

    private void Update()
    {
        DetectTouch();
        
        ProcessTouch();
    }
}