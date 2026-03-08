using System;
using UnityEditor;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    
    private Animator _animator;
    private SwordPlayer player;
    
    private SlowMotion slowMotion;
    public event Action<Transform> HasParry;
    private Transform _target;
   
    
    
    
    private void Start()
    {
        
    player = FindObjectOfType<SwordPlayer>();
    player.RegisterProjectile(this);
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerPlayer Hmp = other.GetComponent<HealthManagerPlayer>();
        ParryManager parry = other.GetComponent<ParryManager>();
            
        if (parry != null && parry.ParryActive)
        {
            Debug.Log("PARRY SUCCESS");
            _animator.SetTrigger("Hit"); // anim ennemi stun 
            slowMotion.StartSlowMotion(0.25f);
            HasParry?.Invoke(_target);

        }
            
        if (Hmp != null)
        {
            Hmp.TakeDamage(1f);
            Debug.Log("TOUCHERRRRRRRRRRRRRRRRR");


        }

    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            player.UnregisterProjectile(this);
        }

    }

    private void Destroy()
    {
        Destroy(gameObject);
    }
    
}
