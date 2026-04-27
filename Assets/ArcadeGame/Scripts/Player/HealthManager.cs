using System;
using UnityEngine;

public class HealthManagerPlayer : MonoBehaviour
{
    [SerializeField] float maxHealth = 5f;
    float health;

    public static event Action OnHealthChanged;

    public float CurrentHealth => health;
    public float MaxHealth => maxHealth;

    private Animator _animator;

    [SerializeField] private LayerMask damageableLayers;
    [SerializeField] private FeedbackConfig feedbackConfig;

    private void Start()
    {
        health = maxHealth;
        _animator = GetComponent<Animator>();

        OnHealthChanged?.Invoke(); // initialise l'UI au lancement
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health,0,maxHealth);

        OnHealthChanged?.Invoke();

        _animator.SetTrigger("Hurt");

        HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);

        if (health <= 0)
            Die();
    }

    private void Die()
    {
        _animator.SetTrigger("Die");
        Debug.Log("Mort");
    }
}