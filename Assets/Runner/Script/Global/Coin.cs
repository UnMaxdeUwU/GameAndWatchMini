using System;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public static event Action OnCoinCollected;

    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float collectDistance = 0.2f;
    [SerializeField] private AudioClip coinSound;

    private Transform _target;
    private bool _isAttracted;

    private void Update()
    {
        if (!_isAttracted || _target == null) return;

        transform.position = Vector3.MoveTowards(
            transform.position,
            _target.position,
            moveSpeed * Time.deltaTime
        );

        if (Vector2.Distance(transform.position, _target.position) <= collectDistance)
        {
            Collect();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isAttracted) return;

        if (other.GetComponent<InputManagerCustomRunner>() != null)
        {
            _target = other.transform;
            _isAttracted = true;
        }
    }

    private void Collect()
    {
        OnCoinCollected?.Invoke();
        SoundFXManager.instance.PlaySound(coinSound, transform, 0.4f);
        Destroy(gameObject);
    }
}