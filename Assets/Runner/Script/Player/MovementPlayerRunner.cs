using UnityEngine;
using System.Collections;

public class MovementPlayerRunner : MonoBehaviour
{
    private float speed = 5f;

    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float jumpBoostForce = 6f;
    [SerializeField] private float jumpBosstDuration = 0.08f;
    [SerializeField] private float maxJumpHeight = 3f;

    private float jumpStartY;
    private bool isBosstingJump;
        
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float fallMultiplier = 2.5f;

    [SerializeField] private float timeAirDuration = 0.2f;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckDistance = 0.2f;
    

    [SerializeField] private float apexHangTime = 0.1f;
    [SerializeField] private float apexThreshold = 0.1f;
    
    
    [SerializeField] InputManagerCustomRunner _inputManager;

    private Animator _animator;
    private Rigidbody2D _rb;

    private bool isGrounded;
    private bool isInTimeAir;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        _inputManager.OnSwipeDown += Fall;
        _inputManager.OnSwipeUp += Jump;
    }

    private void OnDisable()
    {
        _inputManager.OnSwipeDown -= Fall;
        _inputManager.OnSwipeUp -= Jump;
    }

    private void Update()
    {
        CheckGround();
        MoveForward();
        UpdateAnimator();
        RunnerJumpPhysics();
        ClampJumpHeight();
    }

    private void MoveForward()
    {
        _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
    }

    [SerializeField] private float groundCheckRadius = 0.2f;

    private void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }

    private void UpdateAnimator()
    {
        bool isFalling = _rb.linearVelocity.y < -0.1f && !isGrounded;
        _animator.SetBool("IsFalling", isFalling);
    }

    private void Jump()
    {
        if (!isGrounded) return;
        Debug.Log("jump");

        jumpStartY = transform.position.y;
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);

        _animator.SetTrigger("jump");

        StartCoroutine(JumpBoost());
    }

    private IEnumerator JumpBoost()
    {
        isBosstingJump = true;
        float timer = 0f;

        while (timer < jumpBosstDuration)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpBoostForce);
            timer += Time.deltaTime;
            yield return null;
        }

        isBosstingJump = false;
    }
    
    private void ClampJumpHeight()
    {
        float currentJumpHeight = transform.position.y - jumpStartY;
    
        if (currentJumpHeight >= maxJumpHeight && _rb.linearVelocity.y > 0)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        }
    }
    private IEnumerator TimeAir()
    {
        isInTimeAir = true;

        float originalGravity = _rb.gravityScale;
        _rb.gravityScale = originalGravity * 0.5f;

        yield return new WaitForSeconds(timeAirDuration);

        _rb.gravityScale = originalGravity;
        isInTimeAir = false;
    }

    private void Fall()
    {
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier;
        }
    }

    private void Dash()
    {
        _animator.SetTrigger("dash");

        _rb.linearVelocity = new Vector2(dashForce, _rb.linearVelocity.y);
    }
    
    
    private void RunnerJumpPhysics()
    {
        // chute rapide
        if (_rb.linearVelocity.y < 0)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        // petit ralentissement au sommet du saut
        if (Mathf.Abs(_rb.linearVelocity.y) < apexThreshold)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _rb.linearVelocity.y * 0.8f);
        }
    }
}