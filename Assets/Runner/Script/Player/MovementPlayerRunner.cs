using UnityEngine;
using System.Collections;

public class MovementPlayerRunner : MonoBehaviour
{
    private float speed = 6f;

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

    [SerializeField] private float _freezeTime = 1f;
    [SerializeField] private int maxAirFreezes = 2;
    private int airFreezeCount;

    [SerializeField] private float dashCooldown = 1.5f;
    private bool canDash = true;
    private bool hasDashedInAir = false;

    private float originalGravity; 
    
    private Vector2 LastSafePosition;
    private Vector3 _safeDistance = new Vector3(5f, 0);
    
    
    [SerializeField] InputManagerCustomRunner _inputManager;

    private Animator _animator;
    private Rigidbody2D _rb;

    private bool isGrounded;
    private bool isInTimeAir;
    private bool isDashing;
    private bool isFrozen;
    private bool isWaitingForTap = true;
    
    [SerializeField] private float invincibilityTime = 1f;
    private bool isInvincible;
    
    
    [Header("Sound FX")]
    [SerializeField] private AudioClip dashSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip hurtSound;
    [SerializeField] private AudioClip preDeathSound;
    [SerializeField] private AudioClip fallingSound;

    void Start()
    {
        _animator = GetComponent<Animator>();
        _rb = GetComponent<Rigidbody2D>();
        LastSafePosition = transform.position;
        originalGravity =  _rb.gravityScale;
    }

    private void OnEnable()
    {
        _inputManager.OnSwipeDown += Fall;
        _inputManager.OnSwipeUp += Jump;
        _inputManager.OnSwipeRight += Dash;
        _inputManager.OnTape += StartRun;
        HealthManagerPlayer.OnHealthChanged += OnHit;
        Slam.HasAttack += OnAttack;
        CollisionObstacle.PlayerFallInVoid += Respawn;
        
    }

    private void OnDisable()
    {
        _inputManager.OnSwipeDown -= Fall;
        _inputManager.OnSwipeUp -= Jump;
        _inputManager.OnSwipeRight -= Dash;
        HealthManagerPlayer.OnHealthChanged -=  OnHit;
        Slam.HasAttack -= OnAttack;
        CollisionObstacle.PlayerFallInVoid -= Respawn;
        _inputManager.OnTape -= StartRun;
    }

    private void OnAttack()
    {
        if (isGrounded) return;
        if (isDashing) return;
        if (isFrozen) return;
        if (airFreezeCount >= maxAirFreezes) return;
        
        airFreezeCount++;
        StartCoroutine(FreezeAir());
    }

    private void Respawn()
    {

        StartCoroutine(RespawnPlayer());
    }

    private void StartRun()
    {
        if(!isWaitingForTap) return;
        
        isWaitingForTap = false;
        _animator.SetBool("nothing", false);
        //Debug.Log(isWaitingForTap);
    }


    private void Update()
    {
        CheckGround();
        MoveForward();
        UpdateAnimator();
        RunnerJumpPhysics();
        ClampJumpHeight();
    }

    private void OnHit()
    {
        if (isInvincible) return;

        StartCoroutine(Invincibility());

        SoundFXManager.instance.PlaySound(hurtSound, transform, 1f);
        _animator.SetTrigger("hurt");
    }
    
    private IEnumerator Invincibility()
    {
        isInvincible = true;

        yield return new WaitForSeconds(invincibilityTime);

        isInvincible = false;
    }

    private void MoveForward()
    {

        if (isWaitingForTap)
        {
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            return;
        }
        if (!isDashing)
        {
            _rb.linearVelocity = new Vector2(speed, _rb.linearVelocity.y);
        }

    }

    [SerializeField] private float groundCheckRadius = 0.2f;

    private void CheckGround()
    {
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (isGrounded)
        {
            LastSafePosition = transform.position - _safeDistance;
            _animator.SetBool("IsFalling", false);
        }
        
        if (isGrounded && !wasGrounded)
        {
            airFreezeCount = 0;
            hasDashedInAir = false;
        }

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
        //Debug.Log("jump");
        SoundFXManager.instance.PlaySound(jumpSound, transform, 1f);
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
            _rb.linearVelocity += Vector2.up * jumpBoostForce * Time.deltaTime;

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
        
        _rb.gravityScale = originalGravity * 0.5f;

        yield return new WaitForSeconds(timeAirDuration);

        _rb.gravityScale = originalGravity;
        isInTimeAir = false;
        isDashing = false;
    }
    
    private IEnumerator FreezeAir()
    {
        isFrozen = true;
        
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, 0);
        _rb.gravityScale = 0f;

        yield return new WaitForSeconds(_freezeTime);

        _rb.gravityScale = originalGravity;

        isFrozen = false;
    }

    IEnumerator RespawnPlayer()
    {
        _rb.linearVelocity = Vector2.zero;
        _rb.gravityScale = 1f;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        _animator.SetBool("nothing", true);

        isDashing = false;
        isFrozen = false;
        transform.position = LastSafePosition + Vector2.up * 1.5f;
        SoundFXManager.instance.PlaySound(preDeathSound, transform, 1f);
        yield return new WaitForSeconds(0.5f);
        isWaitingForTap = true;
    }
    
    

    private void Fall()
    {
        if (isBosstingJump)
        {
            StopCoroutine(JumpBoost());
            isBosstingJump = false;
        }

        if (_rb.linearVelocity.y < 0)
        {
            SoundFXManager.instance.PlaySound(fallingSound, transform, 1f);
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y * fallMultiplier;
        }
    }
    

    private void Dash()
    {
        if (isWaitingForTap) return;
        if (!canDash) return;

        if (!isGrounded && hasDashedInAir) return;

        SoundFXManager.instance.PlaySound(dashSound, transform, 1f);
        isDashing = true;
        canDash = false;

        if (!isGrounded)
        {
            hasDashedInAir = true;
        }
        _animator.SetTrigger("dash");
        _rb.linearVelocity = new Vector2(dashForce, _rb.linearVelocity.y);
        StartCoroutine(TimeAir());
        StartCoroutine(DashCooldown());
    }

    private IEnumerator DashCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
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