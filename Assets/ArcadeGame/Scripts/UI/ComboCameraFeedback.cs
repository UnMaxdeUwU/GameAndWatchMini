using UnityEngine;

/// <summary>
/// Intercepte chaque hit de combo et applique un HitStop + CameraShake
/// dont l'intensité monte avec le combo.
/// Remplace l'appel direct dans SwordPlayer — branche ici, retire de SwordPlayer.
/// </summary>
public class ComboCameraFeedback : MonoBehaviour
{
    [SerializeField] private ComboConfig _config;
    [SerializeField] private ComboManager _comboManager;

    private void OnEnable()  => ComboManager.OnComboIncreased += HandleHit;
    private void OnDisable() => ComboManager.OnComboIncreased -= HandleHit;

    private void HandleHit(int combo)
    {
        float t = Mathf.Clamp01((float)combo / _config.hitStopMaxCombo);

        float stopDuration = Mathf.Lerp(_config.baseHitStop, _config.maxHitStop, t);
        float shakeMag     = Mathf.Lerp(_config.baseShakeMag, _config.maxShakeMag, t);
        float shakeDur     = Mathf.Lerp(_config.baseShakeDur, _config.maxShakeDur, t);

        HitStop.Instance?.Stop(stopDuration);
        CameraShake.Instance?.Shake(shakeDur, shakeMag);
    }
}
