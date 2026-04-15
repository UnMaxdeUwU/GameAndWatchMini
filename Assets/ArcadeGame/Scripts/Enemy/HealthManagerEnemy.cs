using System;
using UnityEngine;

public class HealthManagerEnemy : MonoBehaviour
{
    [SerializeField] float health = 5f;
    private Animator _animator;
    [SerializeField] private FeedbackConfig feedbackConfig;
    
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
        
        if (health <= 0)
        {
            Die();
            _animator.SetTrigger("Death");
        }
    }

    private void Die()
    {
        Debug.Log(gameObject.name);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        OnEnemyKilled?.Invoke();
    }
}
