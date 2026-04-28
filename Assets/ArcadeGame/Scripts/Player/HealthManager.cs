using System;
using UnityEngine;

public class HealthManagerPlayer : MonoBehaviour
{
    [SerializeField] float maxHealth = 5f;
    float health;

    public static event Action OnHealthChanged;

    /// <summary>
    /// Fired once when the player's health reaches zero.
    /// </summary>
    public static event Action OnPlayerDied;

    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    private Animator _animator;
    private bool _isDead;

    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private FeedbackConfig feedbackConfig;

    private void Start()
    {
        health = maxHealth;
        _isDead = false;
        _animator = GetComponent<Animator>();

        OnHealthChanged?.Invoke(); // initialise l'UI au lancement
    }

    public void TakeDamage(float damage)
    {
        // Ignore tout dégât après la mort — empêche les appels multiples à Die()
        if (_isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke();

        _animator.SetTrigger("Hurt");

        HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        AudioEvents.RaisePlayerDeath();
        OnPlayerDied?.Invoke();
    }
}