using UnityEngine;

/// <summary>
/// À placer sur le child GO qui porte le Collider2D trigger d'attaque de l'ennemi.
/// Isolé du parent pour éviter que l'ennemi se touche lui-même.
/// </summary>
public class EnemyHitbox : MonoBehaviour
{
    // Rempli auto depuis le parent — rien à assigner dans l'inspecteur
    private SworldEnemy _enemy;

    private void Start()
    {
        _enemy = GetComponentInParent<SworldEnemy>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _enemy?.OnHitboxTrigger(other);
    }
}
