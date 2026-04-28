using UnityEngine;
using System.Collections;
using System;

public class Movement_Boss : MonoBehaviour
{
    [Header("Déplacement")]
    [SerializeField] private float distanceBetweenPlayer = 2.0f;
    [SerializeField] private float speed = 3f;

    [Header("Phase 1 - Laser")]
    [SerializeField] private float laserAttackCooldown = 4f;

    [Header("Phase 2 - Mêlée")]
    [SerializeField] private float meleeAttackCooldown = 2.5f;

    public event Action Ataque;

    private Transform _target;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Coroutine _attackRoutine;

    private bool _phase2 = false;

    private ParryManager _parryManager;
    [SerializeField] private Collider2D _collider;
    private SlowMotion slowMotion;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("AsSpawn");

        _attackRoutine = StartCoroutine(LaserLoop());
        _parryManager = FindObjectOfType<ParryManager>();
        slowMotion = FindObjectOfType<SlowMotion>();
    }

    private void OnEnable()
    {
        HealthManagerPlayer.OnPlayerDied += OnPlayerDied;
    }

    private void OnDisable()
    {
        HealthManagerPlayer.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        StopAllCoroutines();
        _attackRoutine = null;
        _rb.linearVelocity = Vector2.zero;
        enabled = false;
    }

    public void EnterPhase2()
    {
        if (_phase2) return;
        _phase2 = true;

        Debug.Log("Boss Phase 2 !");

        if (_attackRoutine != null)
            StopCoroutine(_attackRoutine);

        _attackRoutine = StartCoroutine(MeleeLoop());
    }

    private void FixedUpdate()
    {
        if (!_phase2) return;

        float distance = Vector2.Distance(_rb.position, _target.position);

        if (distance > distanceBetweenPlayer)
        {
            Vector2 direction = (_target.position - _rb.transform.position).normalized;
            _rb.linearVelocity = direction * speed;
            _animator.SetBool("CanRun", true);
        }
        else
        {
            _rb.linearVelocity = Vector2.zero;
            _rb.angularVelocity = 0f;
            _animator.SetBool("CanRun", false);
        }
    }

    private IEnumerator LaserLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(laserAttackCooldown);
            Ataque?.Invoke();
        }
    }

    private IEnumerator MeleeLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(meleeAttackCooldown);

            float distance = Vector2.Distance(_rb.position, _target.position);
            if (distance <= distanceBetweenPlayer)
                TriggerRandomMeleeAttack();
        }
    }

    private void TriggerRandomMeleeAttack()
    {
        if (UnityEngine.Random.value < 0.5f)
            _animator.SetTrigger("FirePunch");
        else
            _animator.SetTrigger("FireSlam");
    }

    /// <summary>Appelé par l'event animator — active la hitbox et joue le son d'attaque mêlée.</summary>
    public void ActiveBox()
    {
        _collider.enabled = true;
        AudioEvents.RaiseBossAttackAlt();
    }

    public void InactiveBox() => _collider.enabled = false;

    // ── Appelé par BossHitbox (child GO) — plus de OnTriggerEnter2D ici ────
    public void OnHitboxTrigger(Collider2D other)
    {
        HealthManagerPlayer hmp = other.GetComponent<HealthManagerPlayer>()
                               ?? other.GetComponentInParent<HealthManagerPlayer>();

        ParryManager parry = other.GetComponent<ParryManager>()
                          ?? other.GetComponentInParent<ParryManager>();

        if (parry != null && parry.ParryActive)
        {
            Debug.Log("PARRY SUCCESS BOSS");
            _animator.SetTrigger("Hit");
            _parryManager?.OnSuccessfulParry(_collider);
            slowMotion?.FreezeFrame(0.25f, 0.15f, 0.35f);
            return;
        }

        if (hmp != null)
            hmp.TakeDamage(1f);
    }
    // ────────────────────────────────────────────────────────────────────────
}