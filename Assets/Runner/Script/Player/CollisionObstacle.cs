using System;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionObstacle : MonoBehaviour
{
    [SerializeField] private HealthManagerPlayer _healthManagerPlayer;
    public static event Action PlayerFallInVoid; 
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.GetComponent<Spike>() != null)
        {
            _healthManagerPlayer.TakeDamage(1);
        }
        else if (other.gameObject.GetComponent<Void>() != null)
        {
            _healthManagerPlayer.TakeDamage(1);
            PlayerFallInVoid?.Invoke();
        }

    }
    
    
}
