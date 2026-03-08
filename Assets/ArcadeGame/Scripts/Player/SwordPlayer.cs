using System;
using UnityEngine;
using System.Collections;

public class SwordPlayer : MonoBehaviour
{
    [SerializeField] private Collider2D col;
    [SerializeField] private InputPlayerManagerCustom inputManager;

    

    [SerializeField] private float baseCooldown = 1.5f;
    [SerializeField] private float minCooldown = 0.1f;
    [SerializeField] private float comboGain = 0.4f;

    private float currentCooldown;
    private bool canAttack = true;
    private float animationSpeed = 1f;
    
    private Animator animator;
    
    public event Action AddCombo;

    private Vector3 StartPosition;
    private Vector3 _counterPosition;

    private float _damage = 1f;

    void Start()
    {
        col = GetComponent<Collider2D>();
        currentCooldown = baseCooldown;
        col.enabled = false;
        animator = GetComponent<Animator>();
        StartPosition = transform.position;
    }

    private void OnEnable()
    {
        inputManager.OnTape += Attack;
    }

    private void OnDisable()
    {
        inputManager.OnTape -= Attack;

    }
    
    public void RegisterEnemy(SworldEnemy enemy)
    {
        enemy.HasParry += Counter;
    }
    
    public void UnregisterEnemy(SworldEnemy enemy)
    {
        enemy.HasParry -= Counter;
    }

    public void RegisterProjectile(Projectile projectile)
    {
        projectile.HasParry += CounterProjectile;
    }

    public void UnregisterProjectile(Projectile projectile)
    {
        projectile.HasParry -= CounterProjectile;
    }
    
    
    

    private void Attack()
    {
        if (!canAttack) return;

        StartCoroutine(AttackRoutine());
        Debug.Log("Attaque!!!!!");
        animator.SetTrigger("Slash");
    }

    IEnumerator AttackRoutine()
    {
        canAttack = false;
        //col.enabled = true;

        yield return new WaitForSeconds(0.1f); 

        //col.enabled = false;
        yield return new WaitForSeconds(currentCooldown);

        canAttack = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HealthManagerEnemy healthManager = other.GetComponent<HealthManagerEnemy>();

        if (healthManager != null)
        {
            healthManager.TakeDamage(_damage);
            
            currentCooldown -= comboGain;
            currentCooldown = Mathf.Clamp(currentCooldown, minCooldown, baseCooldown);
            animationSpeed += 0.1f;
            animationSpeed = Mathf.Clamp(animationSpeed, 1f, 2.5f);
            animator.SetFloat("Speed", animationSpeed );
            
            
            UpCombo(); //ajout combo si enemy touché


        }
    }

    public void CollisionActivate()
    {
        col.enabled = true;
    }

    
    public void CollisionDesactivate()
    {
        col.enabled = false;
    }

    public void ResetCooldownAndSpeed()
    {
        animationSpeed = 1f;
        currentCooldown = 1f;

    }

    public void UpCombo()
    {
        AddCombo?.Invoke();
    }

    private void Counter(Transform point)
    {
        //StartCoroutine(CounterAttack(point));
        animator.SetTrigger("HasParry");
        _counterPosition = point.position;
    }

    private void CounterProjectile(Transform point)
    {
        return;
    }

    /*IEnumerator CounterAttack(Transform point)
    {
        animator.SetTrigger("HasParry");
        yield return new WaitForSeconds(3);
        transform.position = point.position;
        AddCombo?.Invoke();
        yield return new WaitForSecondsRealtime(1f);
        transform.position = StartPosition;
        
    }*/

    public void EndSupriseJump()
    {
        Debug.LogWarning("TP pos on enemy");
        transform.position = _counterPosition;
        _damage = 3f;
        col.offset = new Vector2(0.012f, 0.09f);
        GetComponent<BoxCollider2D>().size = new Vector2(0.35f, 0.37f);
    }

    public void EndAttackDown()
    {
        Debug.LogWarning("TP pos initial");
        AddCombo?.Invoke();
        transform.position = StartPosition;
        _damage = 1f;
        GetComponent<BoxCollider2D>().size = new Vector2(0.2364149f, 0.16f);
    }
    
    
}