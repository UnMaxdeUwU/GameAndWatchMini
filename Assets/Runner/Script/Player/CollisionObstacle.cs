using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionObstacle : MonoBehaviour
{
    [SerializeField] private HealthManagerPlayer _healthManagerPlayer;
    private Rigidbody2D _rigidbody2D;
    public static event Action PlayerFallInVoid;

    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Spike>() != null)
        {
            _healthManagerPlayer.TakeDamage(1);
            _rigidbody2D.position += Vector2.left * 1.5f;
        }
        else if (other.gameObject.GetComponent<Void>() != null)
        {
            _healthManagerPlayer.TakeDamage(1);
            PlayerFallInVoid?.Invoke();
        }

    }
    
    
}
