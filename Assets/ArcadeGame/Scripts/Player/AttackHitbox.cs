using UnityEngine;

/// <summary>
/// Reçoit les OnTriggerEnter2D isolément du parent, évitant les faux positifs.
/// </summary>
public class AttackHitbox : MonoBehaviour
{
    // Référence vers le SwordPlayer parent, assignée auto dans Start
    private SwordPlayer _swordPlayer;

    private void Start()
    {
        _swordPlayer = GetComponentInParent<SwordPlayer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _swordPlayer.TryHitEnemy(other);
        _swordPlayer.TryHitBoss(other);
    }
}
