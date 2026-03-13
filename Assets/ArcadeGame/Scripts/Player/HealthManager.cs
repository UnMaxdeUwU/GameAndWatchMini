using System;
using UnityEngine;

public class HealthManagerPlayer : MonoBehaviour
{
    [SerializeField] float health = 5f;
    public static event Action OnHealthChanged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void TakeDamage(float damage)
    {
        health -= damage;
        OnHealthChanged?.Invoke();
        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Mort");
    }
}
