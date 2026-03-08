using System;
using UnityEngine;
using System.Collections;
public class Enemy_Distance : MonoBehaviour
{
    [SerializeField] private Movement_enemy _enemy;
    private SwordPlayer player;
    
    private SlowMotion slowMotion;
    private Collider2D _collider;
    private bool canAttack = true;
    private Animator _animator;

    private float _cooldown = 3.0f;


    [SerializeField] private Transform _ProjectileSpawnPoint;

    [SerializeField] private GameObject _projectile;
    private Rigidbody2D _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<Collider2D>();
        _animator = GetComponent<Animator>();
        slowMotion = FindObjectOfType<SlowMotion>();
    }

    private void OnEnable()
    {
        _enemy.Ataque += Attack;
    }

    private void OnDisable()
    {
        _enemy.Ataque -= Attack;
    }
    

    private void Attack()
    {
        if (!canAttack) return;

        _animator.SetTrigger("Flame Wawe");
        StartCoroutine(AttackRoutine());
        
        Debug.Log("Attaque!!!!!");

    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        //collider.enabled = true;

        yield return new WaitForSeconds(0.1f); 

        //_collider.enabled = false;
        yield return new WaitForSeconds(_cooldown);

        canAttack = true;
    }

    public void SpawnProjectile()
    {
        Instantiate(_projectile, _ProjectileSpawnPoint.position, _ProjectileSpawnPoint.rotation);
    }

    /*
    public void InactiveBox()
    {
        _collider.enabled = false;
    }
    */
    
}
