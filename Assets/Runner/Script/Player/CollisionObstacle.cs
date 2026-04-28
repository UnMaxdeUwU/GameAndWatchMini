using System;
using UnityEngine;

public class CollisionObstacle : MonoBehaviour
{
    [SerializeField] private HealthManagerRunner _healthManagerRunner;
    private Rigidbody2D _rigidbody2D;
    public static event Action PlayerFallInVoid;
    public static event Action Checkpoint;
    public static event Action Spike;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Spike>() != null)
        {
            _healthManagerRunner.TakeDamage(1);
            Spike?.Invoke();

        }
        else if (other.gameObject.GetComponent<Void>() != null)
        {
            _healthManagerRunner.TakeDamage(1);
            PlayerFallInVoid?.Invoke();
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<CheckPoint>() != null)
        {
            Checkpoint?.Invoke();
        }
    }
    
    
}
