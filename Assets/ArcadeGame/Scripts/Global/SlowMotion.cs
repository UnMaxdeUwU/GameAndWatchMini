using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton qui ralentit le temps pour les moments impactants (parry réussi, counter).
/// Utilise le temps non-scalé pour que la coroutine continue pendant le ralenti.
/// La récupération est progressive (smoothstep) pour éviter le saut brutal.
/// </summary>
public class SlowMotion : MonoBehaviour
{
    public static SlowMotion Instance { get; private set; }

    private Coroutine _currentFreeze;
    private float _remainingFreeze;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    /// <summary>
    /// Ralentit le jeu au scale configuré pendant <paramref name="duration"/> secondes,
    /// puis récupère progressivement sur <paramref name="recovery"/> secondes.
    /// Si un ralenti est déjà actif, le plus long gagne.
    /// </summary>
    public void FreezeFrame(float duration, float slowScale = 0.2f, float recovery = 0.3f)
    {
        if (duration <= 0f) return;

        if (_currentFreeze != null)
        {
            if (duration <= _remainingFreeze) return;
            StopCoroutine(_currentFreeze);
        }

        _currentFreeze = StartCoroutine(SlowRoutine(duration, slowScale, recovery));
    }

    /// <summary>Surcharge pratique qui lit les valeurs depuis un SlowMotionConfig.</summary>
    public void FreezeFrame(SlowMotionConfig config)
    {
        if (config == null) return;
        FreezeFrame(config.slowMotionDuration, config.slowMotionScale, config.slowMotionRecovery);
    }

    private IEnumerator SlowRoutine(float duration, float slowScale, float recovery)
    {
        // Phase ralentie
        _remainingFreeze = duration;
        Time.timeScale = slowScale;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _remainingFreeze = duration - elapsed;
            yield return null;
        }

        // Phase de récupération progressive (smoothstep pour éviter le pop)
        float recoveryElapsed = 0f;
        while (recoveryElapsed < recovery)
        {
            recoveryElapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, recoveryElapsed / recovery);
            Time.timeScale = Mathf.Lerp(slowScale, 1f, t);
            yield return null;
        }

        Time.timeScale = 1f;
        _remainingFreeze = 0f;
        _currentFreeze = null;
    }
}