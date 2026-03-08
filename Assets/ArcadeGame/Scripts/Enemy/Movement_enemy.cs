using UnityEngine;
using System.Collections;
using System;
public class Movement_enemy : MonoBehaviour
{
    [SerializeField] Transform _target;
    private float speed = 2f;
    private float _health;
    private Rigidbody2D _rb;

    private bool canattack = true;
    private float _time;
    private float couldown = 5f;
    
    
    [SerializeField] float distanceBetweenPlayer = 2.0f;

    private float enemyDetectDistance = 5.0f;
    [SerializeField] private LayerMask Enemy;

    private bool CanMove = true;
    public event Action Ataque;
    
    private Animator _animator;
    
    private Coroutine attackRoutine;
    

    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _target = GameObject.FindGameObjectWithTag("Player").transform;
        _animator = GetComponent<Animator>();
        _animator.SetTrigger("AsSpawn");

        StartCoroutine(Spawn());
    }

    IEnumerator Spawn()
    {
        CanMove = false;
        yield return new WaitForSeconds(1f);
        CanMove = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        Vector2 direction =  _target.transform.position - _rb.transform.position ;
        float distance = Vector3.Distance(_rb.transform.position, _target.transform.position);
        RaycastHit2D enemyHit = Physics2D.Raycast(transform.position, direction, enemyDetectDistance, Enemy);


        
        if (distance <= distanceBetweenPlayer  && canattack)
        {
            CanMove = false;
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = 0f;
            _time = Time.time + couldown;
            _animator.SetBool("CanRun", false);

            if (attackRoutine == null)
            {
                attackRoutine = StartCoroutine(CanAttack());   
            }
            return;

        }

        if (enemyHit.collider != null && enemyHit.collider.gameObject != gameObject)
        {
            Debug.Log("HIT");
            _rb.linearVelocity = Vector3.zero;
            _rb.angularVelocity = 0f;
            return;
        }

        if (CanMove)
        {
            Move(); 
        }


        
    }

    private void Move()
    {
        Vector2 direction =  _target.transform.position - _rb.transform.position ;
        direction.Normalize();
        _rb.linearVelocity = direction * speed;
        _animator.SetBool("CanRun", true);
        
        //_rb.MovePosition(_rb.position + _rb.linearVelocity * speed * Time.deltaTime);
    }


    IEnumerator CanAttack()
    {
        while (true)
        {
            yield return new WaitForSeconds(4f);
            Ataque?.Invoke();
        }


    }
    
}
