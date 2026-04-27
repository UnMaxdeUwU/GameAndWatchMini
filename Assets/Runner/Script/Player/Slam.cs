using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slam : MonoBehaviour
{
    
    [SerializeField] InputManagerCustomRunner _runner;
    [SerializeField] private Collider2D _collider2D;
    [SerializeField] private float damage = 1;

    [SerializeField] private AudioClip[] attackSounds;
    
    private HashSet<HealthBox> hitTargets = new HashSet<HealthBox>();
    
    public static event Action HasAttack;

    private bool canAttack = true;
    private bool attackActive = false;

    private Animator _animator;

    private void OnEnable()
    {
        _runner.OnTape += Attack;
    }

    private void OnDisable()
    {
        _runner.OnTape -= Attack;
    }

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void Attack()
    {
        if (!canAttack) return;
        hitTargets.Clear();
        
        StartCoroutine(ResetAttack());
        _animator.SetTrigger("attack");
        HasAttack?.Invoke();
    }

    IEnumerator ResetAttack()
    {
        canAttack = false;
        attackActive = true;

        yield return new WaitForSeconds(0.3f);

        attackActive = false;

        yield return new WaitForSeconds(0.1f);

        canAttack = true;

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryHit(other);
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        TryHit(other);
        
    }
    private void TryHit(Collider2D other)
    {
        if (!attackActive) return;

        HealthBox hpbox = other.GetComponent<HealthBox>();

        if (hpbox != null && !hitTargets.Contains(hpbox))
        {
            hpbox.TakeDamage(damage);
            hitTargets.Add(hpbox);
        }
    }

    public void PlaySoundAttack()
    {
        SoundFXManager.instance.PlayRandomSound(attackSounds, transform, 1f);
    }
    
}
