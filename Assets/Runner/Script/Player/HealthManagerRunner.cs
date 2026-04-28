using System;
using UnityEngine;

public class HealthManagerRunner : MonoBehaviour
{
    [SerializeField] private float maxHealth = 5f;
    private float health;

    /// <summary>
    /// Fired whenever the player's health changes (damage or heal).
    /// </summary>
    public static event Action OnHealthChanged;

    /// <summary>
    /// Fired once when the player's health reaches zero.
    /// </summary>
    public static event Action OnPlayerDied;

    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    private Animator _animator;
    private bool _isDead;

    private void Start()
    {
        health = maxHealth;
        _isDead = false;
        _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// Applies damage to the player. Ignored if the player is already dead.
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth);

        OnHealthChanged?.Invoke();

        if (health <= 0)
        {
            Die();
            return;
        }

        _animator.SetTrigger("Hurt");
    }

    private void Die()
    {
        _isDead = true;
        _animator.SetTrigger("Die");
        AudioEvents.RaisePlayerDeath();
        OnPlayerDied?.Invoke();
    }
}
