using System;
using UnityEngine;
using System.Collections;
public class SworldEnemy : MonoBehaviour
{
    [SerializeField] private Movement_enemy _enemy;
    private SwordPlayer player;
    
    private SlowMotion slowMotion;
    private Collider2D _collider;
    private bool canAttack = true;
    private Animator _animator;

    private float _cooldown = 3.0f;

    public event Action<Transform> HasParry;
    [SerializeField] private Transform _counterPosition;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _animator =  GetComponent<Animator>();
        slowMotion = FindObjectOfType<SlowMotion>();
        
        player = FindObjectOfType<SwordPlayer>();
        player.RegisterEnemy(this);
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerPlayer Hmp = other.GetComponent<HealthManagerPlayer>();
        ParryManager parry = other.GetComponent<ParryManager>();
        
        if (parry != null && parry.ParryActive)
        {
            Debug.Log("PARRY SUCCESS");
            _animator.SetTrigger("Hit"); // anim ennemi stun par exemple
            slowMotion.StartSlowMotion(0.25f);
            HasParry?.Invoke(_counterPosition);
        }
        
        if (Hmp != null)
        {
            Hmp.TakeDamage(1f);
        }

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

        _animator.SetTrigger("Attack");
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

    public void ActiveBox()
    {
        _collider.enabled = true;
    }

    public void InactiveBox()
    {
        _collider.enabled = false;
    }
    
    void OnDestroy()
    {
        if (player != null)
        {
            player.UnregisterEnemy(this);
        }

    }


    
    
}
