using System;
using UnityEngine;
using System.Collections;

public class SworldEnemy : MonoBehaviour
{
    [SerializeField] private Movement_enemy _enemy;
    private SwordPlayer player;

    private SlowMotion slowMotion;
    private Collider2D _collider;
    private bool canAttack = true;
    private Animator _animator;

    private float _cooldown = 3.0f;

    public event Action<Transform> HasParry;
    [SerializeField] private Transform _counterPosition;

    // ── Feedback ─────────────────────────────────────────────────────────────
    [SerializeField] private FeedbackConfig feedbackConfig;
    private ParryManager _parryManager;
    private EnemyStun _enemyStun;
    // ────────────────────────────────────────────────────────────────────────

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        slowMotion = FindObjectOfType<SlowMotion>();

        player = FindObjectOfType<SwordPlayer>();
        player.RegisterEnemy(this);

        // ── Récupère automatiquement les singletons de la scène ─────────────
        _parryManager = FindObjectOfType<ParryManager>();
        _enemyStun = GetComponent<EnemyStun>();
        // ────────────────────────────────────────────────────────────────────
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerPlayer hmp = other.GetComponent<HealthManagerPlayer>();
        ParryManager parry = other.GetComponent<ParryManager>();

        if (parry != null && parry.ParryActive)
        {
            Debug.Log("PARRY SUCCESS");
            _animator.SetTrigger("Hit");

            // ── Feedback parry : hitstop + camera shake + stun ──────────────
            _parryManager?.OnSuccessfulParry(_collider);
            // ────────────────────────────────────────────────────────────────

            // ── SlowMotion dramatique sur le parry ──────────────────────────
            slowMotion?.FreezeFrame(feedbackConfig != null
                ? new SlowMotionConfig { slowMotionDuration = 0.25f, slowMotionScale = 0.15f, slowMotionRecovery = 0.35f }
                : null);
            // ────────────────────────────────────────────────────────────────

            // ── Active le counter du joueur ─────────────────────────────────
            HasParry?.Invoke(_counterPosition);
            // ────────────────────────────────────────────────────────────────

            return; // On ne fait pas de dégâts si le parry est réussi
        }

        if (hmp != null)
        {
            hmp.TakeDamage(1f);
        }
    }

    private void OnEnable()
    {
        _enemy.Ataque += Attack;
    }

    private void OnDisable()
    {
        _enemy.Ataque -= Attack;
    }

    private void Attack()
    {
        if (!canAttack) return;

        // ── Ne pas attaquer si l'ennemi est stun ────────────────────────────
        if (_enemyStun != null && _enemyStun.IsStunned) return;
        // ────────────────────────────────────────────────────────────────────

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

    public void ActiveBox()
    {
        _collider.enabled = true;
    }

    public void InactiveBox()
    {
        _collider.enabled = false;
    }

    void OnDestroy()
    {
        if (player != null)
            player.UnregisterEnemy(this);
    }
}