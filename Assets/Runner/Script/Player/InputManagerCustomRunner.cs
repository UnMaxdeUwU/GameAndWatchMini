using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class InputManagerCustomRunner : MonoBehaviour
{
    public event Action OnMoveLeft;
    public event Action OnMoveRight;

    public event Action OnTape;
    public event Action OnSwipeDown;
    public event Action OnSwipeUp;
    public event Action OnSwipeLeft;
    public event Action OnSwipeRight;

    private RunnerInput controls;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;

    [SerializeField] private float minSwipeDistance = 50f;

    private void Awake()
    {
        controls = new RunnerInput();
    }

    private void OnEnable()
    {
        controls.Enable();

        controls.Gameplay.TouchPress.started += StartTouch;
        controls.Gameplay.TouchPress.canceled += EndTouch;
    }

    private void OnDisable()
    {
        controls.Gameplay.TouchPress.started -= StartTouch;
        controls.Gameplay.TouchPress.canceled -= EndTouch;

        controls.Disable();
    }

    private void StartTouch(InputAction.CallbackContext ctx)
    {
        startTouchPosition = controls.Gameplay.TouchPosition.ReadValue<Vector2>();
    }

    private void EndTouch(InputAction.CallbackContext ctx)
    {
        endTouchPosition = controls.Gameplay.TouchPosition.ReadValue<Vector2>();

        DetectSwipe();
    }

    private void DetectSwipe()
    {
        Vector2 delta = endTouchPosition - startTouchPosition;

        if (delta.magnitude < minSwipeDistance)
        {
            OnTape?.Invoke();
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
}