using System;
using UnityEngine;
using System.Collections;
public class Enemy_Distance : MonoBehaviour
{
    [SerializeField] private Movement_Boss _enemy;
    [SerializeField] private Collider2D _collider2D;
    private SwordPlayer player;
    
    private SlowMotion slowMotion;
    private bool canAttack = true;
    private Animator _animator;

    private float _cooldown = 3.0f;
    

    [SerializeField] private Transform _ProjectileSpawnPoint;

    [SerializeField] private GameObject _projectile;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        slowMotion = FindObjectOfType<SlowMotion>();
    }

    private void OnEnable()
    {
        _enemy.Ataque += Attack;
        HealthManagerPlayer.OnPlayerDied += OnPlayerDied;
    }

    private void OnDisable()
    {
        _enemy.Ataque -= Attack;
        HealthManagerPlayer.OnPlayerDied -= OnPlayerDied;
    }

    private void OnPlayerDied()
    {
        StopAllCoroutines();
        enabled = false;
    }

    private void Attack()
    {
        if (!canAttack) return;

        _animator.SetTrigger("Flame Wawe");
        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(0.1f);
        yield return new WaitForSeconds(_cooldown);
        canAttack = true;
    }

    public void SpawnProjectile()
    {
        Instantiate(_projectile, _ProjectileSpawnPoint.position, _ProjectileSpawnPoint.rotation);
        AudioEvents.RaiseLaserSpawn();
    }

    /// <summary>Appelé par l'event animator — active la hitbox et joue le son d'attaque.</summary>
    private void ActivateCollision2D()
    {
        _collider2D.enabled = true;
        AudioEvents.RaiseBossAttack();
    }

    private void DesactiveCollider2D() => _collider2D.enabled = false;

    /*
    public void InactiveBox()
    {
        _collider.enabled = false;
    }
    */
    
}
