using UnityEngine;
using System;
using System.Collections;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.TouchPhase;

//using UnityEngine.InputSystem;
//using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
//using TouchPhase = UnityEngine.InputSystem.TouchPhase;


public class InputPlayerManagerCustom : MonoBehaviour
{
  public event Action OnMoveLeft; // event dispatcher left
  public event Action OnMoveRight; // event dispatcher right

  private float  _tapDuration = 0.2f;
  private float _swipeDuration = 0.5f;
  
  private SpriteRenderer _spriteRenderer;
  private float _tapTimer = 0.0f;
  private float _swipeTimer = 0.0f;
  private bool _isTouching = false;
  private float width = 0.0f;
  private float height = 0.0f;

  
  private Vector2 startPosition;
  private Vector2 endPosition;
  [SerializeField] private float _minimumDistance = 50f;
  
  private RunnerInput controls;
  
  public event Action OnSwip;
  
  public event Action OnTape;
  public event Action OnSwipeDown;
  public event Action OnSwipeUp;
  public event Action OnSwipeLeft;
  public event Action OnSwipeRight;
  
  
  [SerializeField] Collider2D _collider2D;

  //private InputAction _tapAction;

  private void Awake()
  {
    controls = new RunnerInput();
  }

  private void Start()
  {
    width = Screen.width;
    height = Screen.height;
    _spriteRenderer =  GetComponent<SpriteRenderer>();
    
    //_tapAction = InputSystem.actions.FindAction("Tap");
  }

  private void OnEnable()
  {
    controls.Enable();
    controls.Arcade.TouchPress.started += StartTouch;
    controls.Arcade.TouchPress.canceled += EndTouch;
  }

  private void OnDisable()
  {
    controls.Arcade.TouchPress.started -= StartTouch;
    controls.Arcade.TouchPress.canceled -= EndTouch;
    
    controls.Disable();
  }
  
  private void StartTouch(InputAction.CallbackContext ctx)
  {
    startPosition = controls.Arcade.TouchPosition.ReadValue<Vector2>();
  }

  private void EndTouch(InputAction.CallbackContext ctx)
  {
    endPosition = controls.Arcade.TouchPosition.ReadValue<Vector2>();

    DetectSwipe();
  }
  
  private void DetectSwipe()
  {
    Vector2 delta = endPosition - startPosition;

    if (delta.magnitude < _minimumDistance)
    {
      OnTape?.Invoke();
      if (startPosition.x < Screen.width / 2f)
      {
        OnMoveLeft?.Invoke();
      }
      else
      {
        OnMoveRight?.Invoke();
      }
      return;
    }

    delta.Normalize();

    float dotUp = Vector2.Dot(delta, Vector2.up);
    float dotRight = Vector2.Dot(delta, Vector2.right);

    if (Mathf.Abs(dotUp) > Mathf.Abs(dotRight))
    {
      if (dotUp > 0)
        OnSwipeUp?.Invoke();
      else
        OnSwipeDown?.Invoke();
    }
    else
    {
      if (dotRight > 0)
      {
        OnSwipeRight?.Invoke();
        OnMoveRight?.Invoke();
      }
      else
      {
        OnSwipeLeft?.Invoke();
        OnMoveLeft?.Invoke();
      }
    }
  }


  /*public void OnTap()
  {
    Debug.Log("OnTap");
    OnTape?.Invoke();

  }

  private void Update()
  {
    
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
           if ( _swipeTimer <= _swipeDuration )
           {
             OnSwipe();
           }

           
           if (_tapTimer <= _tapDuration)
           {
            Debug.LogWarning($"Tap OK Touch at {firstTouch.position}");
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
           _tapTimer = 0.0f;
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
    _spriteRenderer.flipX = false;
    _collider2D.offset = new Vector2(0.2f, 0.01552004f);
    OnMoveLeft?.Invoke();
  }

  public void MoveRight()
  {
     // appel de l'event dispatcher associé
    _spriteRenderer.flipX = true;
    _collider2D.offset = new Vector2(-0.21f, 0.01552004f);
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
        MoveLeft();
      }
      else
      {
        MoveRight();
      }
    }
  }#1#
  public void OnSwipe()
  {
    if (Vector2.Distance(startPosition, endPosition) > _minimumDistance)
    {
      //Debug.Log(Vector2.Distance(startPosition, endPosition));
      Vector2 delta = endPosition - startPosition;
      delta = delta.normalized;
               
      float dot = Vector2.Dot(delta, Vector2.up);

      if (Mathf.Abs(dot) > 0.4f)
      {
        if (dot > 0.0f)
        {

          Debug.Log("Swipe up");
        }
        else
        {
          Debug.Log("Swipe down");
          OnSwip?.Invoke(); 
        }
      
      }
    }
  }*/

}
         