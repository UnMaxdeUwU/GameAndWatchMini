using System;
using UnityEngine;
using System.Collections;

public class SworldEnemy : MonoBehaviour
{
    [SerializeField] private Movement_enemy _enemy;
    private SwordPlayer player;

    private SlowMotion slowMotion;
    [SerializeField] Collider2D _collider;
    private bool canAttack = true;
    private Animator _animator;

    private float _cooldown = 3.0f;

    public event Action<Transform> HasParry;
    [SerializeField] private Transform _counterPosition;

    [SerializeField] private FeedbackConfig feedbackConfig;
    private ParryManager _parryManager;
    private EnemyStun _enemyStun;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        slowMotion = FindObjectOfType<SlowMotion>();

        player = FindObjectOfType<SwordPlayer>();
        player.RegisterEnemy(this);

        _parryManager = FindObjectOfType<ParryManager>();
        _enemyStun = GetComponent<EnemyStun>();
    }

    // ── Appelé par EnemyHitbox (child GO) — plus de OnTriggerEnter2D ici ───
    public void OnHitboxTrigger(Collider2D other)
    {
        HealthManagerPlayer hmp = other.GetComponent<HealthManagerPlayer>()
                               ?? other.GetComponentInParent<HealthManagerPlayer>();

        ParryManager parry = other.GetComponent<ParryManager>()
                          ?? other.GetComponentInParent<ParryManager>();

        if (parry != null && parry.ParryActive)
        {
            Debug.Log("PARRY SUCCESS");
            _animator.SetTrigger("Hit");
            _parryManager?.OnSuccessfulParry(_collider);
            slowMotion?.FreezeFrame(0.25f, 0.15f, 0.35f);
            HasParry?.Invoke(_counterPosition);
            return;
        }

        if (hmp != null)
            hmp.TakeDamage(1f);
    }
    // ────────────────────────────────────────────────────────────────────────

    private void OnEnable()
    {
        _enemy.Ataque += Attack;
        HealthManagerPlayer.OnPlayerDied += OnPlayerDied;
    }

    private void OnDisable()
    {
        _enemy.Ataque -= Attack;
        HealthManagerPlayer.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        StopAllCoroutines();
        enabled = false;
    }

    private void Attack()
    {
        if (!canAttack) return;
        if (_enemyStun != null && _enemyStun.IsStunned) return;

        _animator.SetTrigger("Attack");
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(_cooldown);
        canAttack = true;
    }

    /// <summary>Appelé par l'event animator — active la hitbox et joue le son d'attaque.</summary>
    public void ActiveBox()
    {
        _collider.enabled = true;
        AudioEvents.RaiseEnemySimpleAttack();
    }

    public void InactiveBox() => _collider.enabled = false;

    void OnDestroy()
    {
        if (player != null)
            player.UnregisterEnemy(this);
    }
}