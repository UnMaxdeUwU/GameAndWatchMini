using System.Collections;
using UnityEngine;

/// <summary>
/// Ajoute ce composant sur chaque ennemi.
/// Appelle Stun(duration) pour interrompre l'ennemi et jouer le feedback visuel.
/// Ce script suspend l'IA de l'ennemi pendant la durée du stun.
/// </summary>
[RequireComponent(typeof(Animator))]
public class EnemyStun : MonoBehaviour
{
    [Header("Références")]
    [SerializeField] private MonoBehaviour[] behavioursToDisableDuringStun; // ex: EnemyAI, EnemyAttack

    [Header("Feedback visuel")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color stunColor = new Color(1f, 0.4f, 0.1f, 1f); // orange choc
    [SerializeField] private int flashCount = 3;

    public bool IsStunned { get; private set; }

    private Animator _animator;
    private Coroutine _stunRoutine;
    private Color _originalColor;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (spriteRenderer != null)
            _originalColor = spriteRenderer.color;
    }

    /// <summary>
    /// Stun l'ennemi pendant <paramref name="duration"/> secondes.
    /// Si déjà stun, prolonge si la nouvelle durée est plus longue.
    /// </summary>
    public void Stun(float duration)
    {
        if (duration <= 0f) return;

        if (_stunRoutine != null)
        {
            StopCoroutine(_stunRoutine);
        }

        _stunRoutine = StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        IsStunned = true;

        // Désactive les comportements d'IA
        foreach (var b in behavioursToDisableDuringStun)
            if (b != null) b.enabled = false;

        // Trigger animation de stun si elle existe
        if (_animator.HasParameter("Stun"))
            _animator.SetTrigger("Stun");

        // Flash de couleur choc
        if (spriteRenderer != null)
        {
            for (int i = 0; i < flashCount; i++)
            {
                spriteRenderer.color = stunColor;
                yield return new WaitForSecondsRealtime(0.04f);
                spriteRenderer.color = _originalColor;
                yield return new WaitForSecondsRealtime(0.04f);
            }
        }

        // Attente du reste du stun
        float flashTime = flashCount * 0.08f;
        float remaining = duration - flashTime;
        if (remaining > 0f)
            yield return new WaitForSecondsRealtime(remaining);

        // Réactive les comportements
        foreach (var b in behavioursToDisableDuringStun)
            if (b != null) b.enabled = true;

        IsStunned = false;
        _stunRoutine = null;
    }

    private void OnDisable()
    {
        // Nettoyage si l'objet est détruit pendant un stun
        if (spriteRenderer != null)
            spriteRenderer.color = _originalColor;

        foreach (var b in behavioursToDisableDuringStun)
            if (b != null) b.enabled = true;

        IsStunned = false;
    }
}

/// <summary>
/// Extension utilitaire pour vérifier si un Animator possède un paramètre donné.
/// </summary>
public static class AnimatorExtensions
{
    public static bool HasParameter(this Animator animator, string paramName)
    {
        foreach (var param in animator.parameters)
            if (param.name == paramName) return true;
        return false;
    }
}
