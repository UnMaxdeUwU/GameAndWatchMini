using UnityEngine;

public class HealthBox : MonoBehaviour
{
    [SerializeField] private float _health;
    
    [SerializeField] AudioClip explosion;
    private Collider2D _collider;
    private Animator _animator;
    

    private void Start()
    {

        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider2D>();
    }
    
    public void TakeDamage(float damage)
    {
        _health -= damage;
        _animator.SetTrigger("hit");
        SoundFXManager.instance.PlaySound(explosion, transform, 1f);
        if (_health <= 0)
        {
            _animator.SetTrigger("dead");

        }
    }

    private void Die()
    {
        Debug.Log("Mort");
        Destroy(gameObject);
    }
}
