using UnityEngine;

public class HealthManagerEnemy : MonoBehaviour
{
    [SerializeField] float health = 5f;
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
            _animator.SetTrigger("Death");
        }
    }

    private void Die()
    {
        Debug.Log("Mort");
        Destroy(gameObject);
    }
}
