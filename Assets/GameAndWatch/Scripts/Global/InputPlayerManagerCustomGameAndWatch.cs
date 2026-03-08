using UnityEngine;
using System;

public class InputPlayerManagerCustomGameAndWatch : MonoBehaviour
{
    public event Action OnMoveLeft;
    public event Action OnMoveRight;
    public event Action OnTape;
    public event Action OnSwip;

    private float _tapDuration = 0.2f;
    private float _swipeDuration = 0.5f;
    

    private float _tapTimer = 0f;
    private float _swipeTimer = 0f;

    private bool _isTouching = false;

    private float width;
    private float height;

    private Vector2 startPosition;
    private Vector2 endPosition;

    private float _minimumDistance = 15f;
    private SpriteRenderer _spriteRenderer;
    
    
    private void Start()
    {
        width = Screen.width;
        height = Screen.height;
        _spriteRenderer =  GetComponent<SpriteRenderer>();
    }

    public void OnTap()
    {
        Debug.Log("OnTap");
        OnTape?.Invoke();
    }

    private void Update()
    {
        if (Input.touchCount == 0)
            return;

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

            if (_swipeTimer <= _swipeDuration)
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

            _tapTimer = 0f;
            _swipeTimer = 0f;
        }

        if (_isTouching)
        {
            _tapTimer += Time.deltaTime;
            _swipeTimer += Time.deltaTime;
        }
    }

    public void MoveLeft()
    {
        OnMoveLeft?.Invoke();
        _spriteRenderer.flipX = false;
    }

    public void MoveRight()
    {
        OnMoveRight?.Invoke();
        _spriteRenderer.flipX = true;
    }

    public void OnSwipe()
    {
        if (Vector2.Distance(startPosition, endPosition) > _minimumDistance)
        {
            Vector2 delta = endPosition - startPosition;
            delta = delta.normalized;

            float dot = Vector2.Dot(delta, Vector2.up);

            if (Mathf.Abs(dot) > 0.4f)
            {
                if (dot > 0)
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
    }
}