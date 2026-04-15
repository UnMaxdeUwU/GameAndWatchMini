using System;
using UnityEditor;
using UnityEngine;

public class ProjectilePlayer : MonoBehaviour
{
    
    
    [SerializeField] private FeedbackConfig feedbackConfig;
    private SworldEnemy _enemy;
    
    private SlowMotion slowMotion;

    [SerializeField] private float slowMotionTime = 3f;
    //public event Action<Transform> HasParry;
    //private Transform _target;
   
    
    
    
    private void Start()
    {
        
        //player = FindObjectOfType<SwordPlayer>();
        //player.RegisterProjectile(this);
        SlowMotion.Instance?.FreezeFrame(slowMotionTime);
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerEnemy Hme = other.GetComponent<HealthManagerEnemy>();
        if (Hme != null)
        {
            Hme.TakeDamage(1f);
            Debug.Log("Enemy hit by projectile");
            Debug.Log("Enemy hit by projectile");
            HitStop.Instance?.Stop(feedbackConfig.hitStopDuration);
            CameraShake.Instance?.Shake(feedbackConfig.hitShakeDuration, feedbackConfig.hitShakeMagnitude);


        }

    }
    

    private void Destroy()
    {
        Destroy(gameObject);
    }
    
}