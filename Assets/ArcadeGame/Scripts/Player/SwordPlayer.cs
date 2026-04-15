using System;
using UnityEngine;
using System.Collections;

public class SwordPlayer : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private InputPlayerManagerCustom inputManager;
    [SerializeField] private GameObject Projectile;

    [SerializeField] private float baseCooldown = 1.5f;
    [SerializeField] private float minCooldown = 0.1f;
    [SerializeField] private float comboGain = 0.4f;

    // ── Feedback ────────────────────────────────────────────────────────────
    [SerializeField] private FeedbackConfig feedbackConfig;
    // ────────────────────────────────────────────────────────────────────────

    private float currentCooldown;
    private bool canAttack = true;
    private float animationSpeed = 1f;

    private Animator animator;

    public event Action AddCombo;

    [SerializeField] private Transform SpawnPointProjectile;
    private Vector3 StartPosition;
    private Vector3 _counterPosition;
    

    private float _damage = 1f;

    void Start()
    {
        col = GetComponent<Collider2D>();
        currentCooldown = baseCooldown;
        col.enabled = false;
        animator = GetComponent<Animator>();
        StartPosition = transform.position;
    }

    private void OnEnable()
    {
        inputManager.OnTape += Attack;
    }

    private void OnDisable()
    {
        inputManager.OnTape -= Attack;
    }

    public void RegisterEnemy(SworldEnemy enemy)
    {
        enemy.HasParry += Counter;
    }

    public void UnregisterEnemy(SworldEnemy enemy)
    {
        enemy.HasParry -= Counter;
    }

    public void RegisterProjectile(Projectile projectile)
    {
        projectile.HasParry += CounterProjectile;
    }

    public void UnregisterProjectile(Projectile projectile)
    {
        projectile.HasParry -= CounterProjectile;
    }

    private void Attack()
    {
        if (!canAttack) return;

        StartCoroutine(AttackRoutine());
        Debug.Log("Attaque!!!!!");
        animator.SetTrigger("Slash");
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;

        yield return new WaitForSeconds(0.1f);

        yield return new WaitForSeconds(currentCooldown);

        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerEnemy healthManager = other.GetComponent<HealthManagerEnemy>();

        if (healthManager != null)
        {
            healthManager.TakeDamage(_damage);

            currentCooldown -= comboGain;
            currentCooldown = Mathf.Clamp(currentCooldown, minCooldown, baseCooldown);
            animationSpeed += 0.1f;
            animationSpeed = Mathf.Clamp(animationSpeed, 1f, 2.5f);
            animator.SetFloat("Speed", animationSpeed);

            // ── Feedback : HitStop + Camera Shake ───────────────────────────
            if (feedbackConfig != null)
            {
                HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);
                CameraShake.Instance?.Shake(feedbackConfig.hitShakeDuration, feedbackConfig.hitShakeMagnitude);
            }
            // ────────────────────────────────────────────────────────────────

            // ── Feedback : Stun sur l'ennemi touché ─────────────────────────
            EnemyStun stun = other.GetComponent<EnemyStun>();
            if (stun == null) stun = other.GetComponentInParent<EnemyStun>();
            if (stun != null && feedbackConfig != null)
                stun.Stun(feedbackConfig.stunDuration);
            // ────────────────────────────────────────────────────────────────

            UpCombo();
        }
    }

    public void CollisionActivate()
    {
        col.enabled = true;
    }

    public void CollisionDesactivate()
    {
        col.enabled = false;
    }

    public void ResetCooldownAndSpeed()
    {
        animationSpeed = 1f;
        currentCooldown = 1f;
    }

    public void UpCombo()
    {
        AddCombo?.Invoke();
    }

    private void Counter(Transform point)
    {
        animator.SetTrigger("HasParry");
        //_counterPosition = point.position;

        // ── Feedback : hitstop + shake plus intenses pour le counter ────────
        if (feedbackConfig != null)
        {
            HitStop.Instance?.Stop(feedbackConfig.counterStopDuration);
            CameraShake.Instance?.Shake(feedbackConfig.counterShakeDuration, feedbackConfig.counterShakeMagnitude);
        }
        // ────────────────────────────────────────────────────────────────────
    }

    private void CounterProjectile(Transform point)
    {
        Instantiate(Projectile, SpawnPointProjectile.position, Quaternion.identity);
    }

    public void EndSupriseJump()
    {
        Debug.LogWarning("TP pos on enemy");
        transform.position = _counterPosition;
        _damage = 3f;
        col.offset = new Vector2(0.012f, 0.09f);
        GetComponent<BoxCollider2D>().size = new Vector2(0.35f, 0.37f);
    }

    public void EndAttackDown()
    {
        Debug.LogWarning("TP pos initial");
        AddCombo?.Invoke();
        transform.position = StartPosition;
        _damage = 1f;
        GetComponent<BoxCollider2D>().size = new Vector2(0.2364149f, 0.16f);
    }
}