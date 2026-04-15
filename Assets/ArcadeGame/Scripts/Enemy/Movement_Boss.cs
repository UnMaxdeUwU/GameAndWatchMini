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

    // Event écouté par Enemy_Distance pour spawn le projectile (phase 1)
    public event Action Ataque;

    private Transform _target;
    private Rigidbody2D _rb;
    private Animator _animator;
    private Coroutine _attackRoutine;

    private bool _phase2 = false;

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("AsSpawn");

        // Démarre la boucle laser phase 1
        _attackRoutine = StartCoroutine(LaserLoop());
    }

    /// <summary>
    /// Appelé par HealthManagerBoss au premier TakeDamage.
    /// Arrête le laser et démarre le rush mêlée.
    /// </summary>
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

    // ── Phase 1 ─────────────────────────────────────────────────────────────

    private IEnumerator LaserLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(laserAttackCooldown);
            Ataque?.Invoke(); // Enemy_Distance écoute ça pour spawner le projectile
        }
    }

    // ── Phase 2 ─────────────────────────────────────────────────────────────

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
}
