using UnityEngine;

public class HealthManagerPlayer : MonoBehaviour
{
    [SerializeField] float health = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void TakeDamage(float damage)
    {
        health -= damage;
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
