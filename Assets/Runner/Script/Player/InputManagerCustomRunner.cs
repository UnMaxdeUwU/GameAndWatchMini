using UnityEngine;
using System;
using System.Collections;

public class InputManagerCustomRunner : MonoBehaviour
{
  public event Action OnMoveLeft; // event dispatcher left
  public event Action OnMoveRight; // event dispatcher right

  [SerializeField] private float _tapDuration = 0.2f;
  [SerializeField] float _swipeDuration = 0.5f;
  [SerializeField] float _minimumDistance = 15f;
  
  private SpriteRenderer _spriteRenderer;
  private float _tapTimer = 0.0f;
  private float _swipeTimer = 0.0f;
  private bool _isTouching = false;
  private float width = 0.0f;
  private float height = 0.0f;


  private Vector2 startPosition;
  private Vector2 endPosition;
  public event Action OnTape;
  public event Action OnSwipeDown;
  public event Action OnSwipeUp;
  public event Action OnSwipeLeft;
  public event Action OnSwipeRight;




  [SerializeField] Collider2D _collider2D;

  //private InputAction _tapAction;

  private void Start()
  {
    width = Screen.width;
    height = Screen.height;
    _spriteRenderer = GetComponent<SpriteRenderer>();

    //_tapAction = InputSystem.actions.FindAction("Tap");
  }


  public void OnTap()
  {
    Debug.Log("OnTap");
    OnTape?.Invoke();

  }

  private void Update()
  {
    /*if (Touch.activeTouches.Count <= 0)
    {
      return;
    }
    Touch touch = _tapAction.ReadValue<Touch>();
    if (touch.phase == TouchPhase.Began)
    {
      startPosition = touch.screenPosition;
    }
    else if (touch.phase == TouchPhase.Moved)
    {
      endPosition = touch.screenPosition;
      OnSwipe();
    }*/

    if (Input.touchCount > 0)
    {
      Touch firstTouch = Input.GetTouch(0);

      if (firstTouch.phase == TouchPhase.Began)
      {
        _isTouching = true;
        startPosition = firstTouch.position;
      }
      else if (firstTouch.phase == TouchPhase.Ended)
      {
        _isTouching = false;
        endPosition = firstTouch.position;

        float distance = Vector2.Distance(startPosition, endPosition);

        if (distance > _minimumDistance)
        {
          OnSwipe();
        }
        else if (_tapTimer <= _tapDuration)
        {
          Debug.Log($"Tap OK Touch at {firstTouch.position}");
          OnTap();

          if (firstTouch.position.x < width / 2)
          {
            MoveRight();
          }
          else
          {
            MoveLeft();
          }
        }

        _tapTimer = 0f;
        _swipeTimer = 0.0f;
      }
      
    }

    if (_isTouching)
    {
      _tapTimer += Time.deltaTime;
      _swipeTimer += Time.deltaTime;

    }


    if (Input.GetKeyDown(KeyCode.RightArrow))
    {
      MoveRight();
    }

    if (Input.GetKeyDown(KeyCode.LeftArrow))
    {
      MoveLeft();
    }

  }



  public void MoveLeft()
  {
    //OnMoveLeft?.Invoke(); // appel de l'event dispatcher associé
    OnMoveLeft?.Invoke();
  }

  public void MoveRight()
  {
    // appel de l'event dispatcher associé
    OnMoveRight?.Invoke();

  }

  /*public void OnSwipe()
  {
    Vector2 delta = endPosition - startPosition;
    delta = delta.normalized;

    float dot = Vector2.Dot(delta, Vector2.right);

    if (Mathf.Abs(dot) > 0.7f)
    {
      if (dot < 0.0f)
      {
          ;   //swipegauche
      }
      else
      {
         ();
      }
    }
  }*/
  public void OnSwipe()
  {
    if (Vector2.Distance(startPosition, endPosition) > _minimumDistance)
    {
      //Debug.Log(Vector2.Distance(startPosition, endPosition));
      Vector2 delta = endPosition - startPosition;
      delta = delta.normalized;

      float dotUp = Vector2.Dot(delta, Vector2.up);
      float dotRight = Vector2.Dot(delta, Vector2.right);

      if (Mathf.Abs(dotUp) > Mathf.Abs(dotRight))
      {
        // Swipe vertical
        if (dotUp > 0)
        {
          Debug.Log("Swipe Up");
          OnSwipeUp?.Invoke();
        }
        else
        {
          Debug.Log("Swipe Down");
          OnSwipeDown?.Invoke();
        }
      }
      else
      {
        // Swipe horizontal
        if (dotRight > 0)
        {
          Debug.Log("Swipe Right");
          OnSwipeRight?.Invoke();
        }
        else
        {
          Debug.Log("Swipe Left");
          OnSwipeLeft?.Invoke();
        }
      }
    }

  }
  
}

