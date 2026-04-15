using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton gérant le HitStop : un freeze très court (30-180ms) qui ponctue les impacts.
/// Distinct du SlowMotion qui gère les ralentis plus longs (parry, counter).
/// Utilise le temps non-scalé pour continuer à tourner pendant le freeze.
/// </summary>
public class HitStop : MonoBehaviour
{
    public static HitStop Instance { get; private set; }

    private Coroutine _stopRoutine;
    private float _remainingStop;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Freeze le jeu pendant <paramref name="duration"/> secondes (unscaled).
    /// Si un stop est déjà actif, le plus long gagne.
    /// </summary>
    public void Stop(float duration)
    {
        if (duration <= 0f) return;

        if (_stopRoutine != null)
        {
            if (duration <= _remainingStop) return;
            StopCoroutine(_stopRoutine);
        }

        _stopRoutine = StartCoroutine(StopRoutine(duration));
    }

    private IEnumerator StopRoutine(float duration)
    {
        _remainingStop = duration;
        Time.timeScale = 0f;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _remainingStop = duration - elapsed;
            yield return null;
        }

        // Récupération progressive plutôt qu'un retour brutal à 1
        float recoveryTime = duration * 0.5f;
        float recoveryElapsed = 0f;
        while (recoveryElapsed < recoveryTime)
        {
            recoveryElapsed += Time.unscaledDeltaTime;
            Time.timeScale = Mathf.Lerp(0f, 1f, recoveryElapsed / recoveryTime);
            yield return null;
        }

        Time.timeScale = 1f;
        _remainingStop = 0f;
        _stopRoutine = null;
    }
}
