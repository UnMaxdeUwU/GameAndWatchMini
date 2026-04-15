using System.Collections;
using UnityEngine;

/// <summary>
/// Singleton attaché à la caméra principale.
/// Appelle CameraShake.Instance.Shake(duration, magnitude) depuis n'importe quel système.
/// Les shakes s'additionnent : plusieurs appels simultanés fusionnent en gardant le plus fort.
/// </summary>
public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    private Vector3 _originalLocalPos;
    private Coroutine _shakeRoutine;
    private float _remainingDuration;
    private float _currentMagnitude;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        _originalLocalPos = transform.localPosition;
    }

    /// <summary>
    /// Lance un shake de caméra. Si un shake est déjà actif, on garde le plus intense.
    /// </summary>
    public void Shake(float duration, float magnitude)
    {
        if (duration <= 0f || magnitude <= 0f) return;

        // Si un shake plus faible est déjà actif, on le remplace
        if (_shakeRoutine != null)
        {
            if (magnitude <= _currentMagnitude && duration <= _remainingDuration) return;
            StopCoroutine(_shakeRoutine);
        }

        _shakeRoutine = StartCoroutine(ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        _currentMagnitude = magnitude;
        _remainingDuration = duration;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            _remainingDuration = duration - elapsed;

            // Atténuation progressive sur la fin du shake
            float progress = elapsed / duration;
            float dampedMagnitude = magnitude * (1f - Mathf.SmoothStep(0f, 1f, progress));

            transform.localPosition = _originalLocalPos + (Vector3)Random.insideUnitCircle * dampedMagnitude;

            yield return null;
        }

        transform.localPosition = _originalLocalPos;
        _shakeRoutine = null;
        _currentMagnitude = 0f;
        _remainingDuration = 0f;
    }
}
