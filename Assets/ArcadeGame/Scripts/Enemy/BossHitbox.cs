using UnityEngine;

/// <summary>
/// À placer sur le child GO qui porte le Collider2D trigger d'attaque du boss.
/// </summary>
public class BossHitbox : MonoBehaviour
{
    private Movement_Boss _boss;

    private void Start()
    {
        _boss = GetComponentInParent<Movement_Boss>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _boss?.OnHitboxTrigger(other);
    }
}
