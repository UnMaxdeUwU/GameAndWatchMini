using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [SerializeField] private float _health;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
        if (_health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Debug.Log("Mort");
        Destroy(gameObject);
    }
}
