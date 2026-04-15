using UnityEngine;
using System.Collections;

public class ParryManager : MonoBehaviour
{
    private Coroutine parryRoutine;

    public bool ParryActive { get; private set; }

    [SerializeField] private float parryWindow = 0.25f;

    // ── Feedback ────────────────────────────────────────────────────────────
    [SerializeField] private FeedbackConfig feedbackConfig;
    // ────────────────────────────────────────────────────────────────────────

    public void StartParryWindow()
    {
        if (parryRoutine != null)
            StopCoroutine(parryRoutine);

        parryRoutine = StartCoroutine(ParryCoroutine());
    }

    /// <summary>
    /// Appelé par l'ennemi quand son attaque est parryée.
    /// Passe l'ennemi en stun et déclenche les feedbacks.
    /// </summary>
    /// <param name="enemyCollider">Le collider de l'ennemi qui a été parryé.</param>
    public void OnSuccessfulParry(Collider2D enemyCollider)
    {
        if (feedbackConfig == null) return;

        // HitStop + Camera shake puissants pour récompenser le parry
        HitStop.Instance?.Stop(feedbackConfig.parryStopDuration);
        CameraShake.Instance?.Shake(feedbackConfig.parryShakeDuration, feedbackConfig.parryShakeMagnitude);

        // Stun prolongé sur l'ennemi parryé
        EnemyStun stun = enemyCollider.GetComponent<EnemyStun>();
        if (stun == null) stun = enemyCollider.GetComponentInParent<EnemyStun>();
        if (stun != null)
            stun.Stun(feedbackConfig.parryStunDuration);
    }

    IEnumerator ParryCoroutine()
    {
        ParryActive = true;
        yield return new WaitForSeconds(parryWindow);
        ParryActive = false;
        parryRoutine = null;
    }
}