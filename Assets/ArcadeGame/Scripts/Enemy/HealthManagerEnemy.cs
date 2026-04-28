using System;
using UnityEngine;

public class HealthManagerEnemy : MonoBehaviour
{
    [SerializeField] float health = 5f;
    private Animator _animator;
    [SerializeField] private FeedbackConfig feedbackConfig;

    // ── Layer autorisé à infliger des dégâts ────────────────────────────────
    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private Collider2D _collider2D;
    
    // ────────────────────────────────────────────────────────────────────────

    public static event Action OnEnemyKilled;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        _animator.SetTrigger("Hit");
        HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);
        AudioEvents.RaiseEnemyHit();

        if (health <= 0)
        {   
            _collider2D.enabled = false;
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Ignore tout collider qui n'est pas dans les layers autorisés
        if ((damageableLayers.value & (1 << other.gameObject.layer)) == 0) return;

        // Le TakeDamage est géré par SwordPlayer / ProjectilePlayer qui appellent
        // directement cette méthode — ce bloc sert uniquement de garde de sécurité
        // si un autre système veut passer par le trigger directement.
    }

    private void Die()
    {
        _animator.SetTrigger("Death");
        Debug.Log(gameObject.name);
        Destroy(gameObject,3f);
    }

    private void OnDestroy()
    {
        OnEnemyKilled?.Invoke();
    }
}