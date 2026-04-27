using UnityEngine;

/// <summary>
/// Active/désactive des VFX sur le joueur selon le tier combo.
/// Chaque tier correspond à un prefab VFX (ParticleSystem ou VFX Graph).
/// 
/// Setup : assigne les VFX dans l'inspecteur dans l'ordre croissant.
///   Tier 0 (5 hits)  : weapon trails
///   Tier 1 (15 hits) : afterimages
///   Tier 2 (30 hits) : aura particles
///   Tier 3 (50 hits) : distortion / overdrive
///   Tier 4 (80 hits) : full overdrive
/// </summary>
public class ComboVFXController : MonoBehaviour
{
    [Tooltip("VFX activés par tier, dans l'ordre croissant des paliers.")]
    [SerializeField] private GameObject[] _tierVFX;

    private int _activeTier = -1;

    private void OnEnable()
    {
        ComboManager.OnRankChanged += HandleRankChanged;
        ComboManager.OnComboReset  += HandleReset;
    }

    private void OnDisable()
    {
        ComboManager.OnRankChanged -= HandleRankChanged;
        ComboManager.OnComboReset  -= HandleReset;
    }

    private void HandleRankChanged(int rankIndex, int combo)
    {
        SetTier(rankIndex);
    }

    private void HandleReset()
    {
        SetTier(-1);
    }

    private void SetTier(int tier)
    {
        if (tier == _activeTier) return;
        _activeTier = tier;

        for (int i = 0; i < _tierVFX.Length; i++)
        {
            if (_tierVFX[i] == null) continue;

            bool shouldBeActive = i <= tier;
            if (_tierVFX[i].activeSelf != shouldBeActive)
                _tierVFX[i].SetActive(shouldBeActive);
        }
    }
}
