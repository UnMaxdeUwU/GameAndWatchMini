using System;
using UnityEngine;

public class HealthManagerBoss : MonoBehaviour
{
    [SerializeField] private float health = 20f;
    [SerializeField] private FeedbackConfig feedbackConfig;
    [SerializeField] private Collider2D _collider2D;
    private Animator _animator;
    private Movement_Boss _movementBoss;
    private bool _phase2Triggered = false;
    public static Action OnEliteKilled;
    

    public static event Action OnBossKilled;
    [SerializeField] private LayerMask damageableLayers;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _movementBoss = GetComponent<Movement_Boss>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        _animator.SetTrigger("Hit");
        HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);
        CameraShake.Instance?.Shake(feedbackConfig.hitShakeDuration, feedbackConfig.hitShakeMagnitude);
        AudioEvents.RaiseBossHit();

        // Premier dégât reçu → passe en phase 2
        if (!_phase2Triggered)
        {
            _phase2Triggered = true;
            _movementBoss.EnterPhase2();
        }

        if (health <= 0)
            Die();
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
        OnEliteKilled?.Invoke();
        _collider2D.enabled = false;
        _animator.SetTrigger("Death");
        OnBossKilled?.Invoke();
        Destroy(gameObject, 3f); // petit délai pour laisser l'anim de mort jouer
    }
}
