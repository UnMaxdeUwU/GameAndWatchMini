using System;
using UnityEngine;

/// <summary>
/// Gère le score global.
/// - +10 par hit ennemi (OnHitScoreAdded)
/// - bonus à l'expiration du combo = hits * multiplicateur de rank
/// Émet OnScoreChanged et OnBonusScoreAdded pour l'UI.
/// </summary>
public class ScoreManager : MonoBehaviour
{
    /// <summary>Static instance for direct read access (e.g. GameOverManager).</summary>
    public static ScoreManager Instance { get; private set; }

    [SerializeField] private ComboConfig _config;

    // Multiplicateurs par rank index (0=BRUTAL … 4=GODLIKE)
    // Plus le rank est haut, plus le multiplicateur est grand
    [SerializeField] private float[] _rankMultipliers = { 1.5f, 2.5f, 4f, 6f, 10f };

    [Tooltip("Score de base par hit avant multiplicateur de rank (utilisé au bonus combo)")]
    [SerializeField] private int _baseHitScore = 100;

    public static event Action<int>        OnScoreChanged;    // score total mis à jour
    public static event Action<int, Color> OnBonusScoreAdded; // (bonus, couleur du rang)

    public int TotalScore { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        ComboManager.OnHitScoreAdded  += HandleHitScore;
        ComboManager.OnComboExpired   += HandleComboExpired;
    }

    private void OnDisable()
    {
        ComboManager.OnHitScoreAdded  -= HandleHitScore;
        ComboManager.OnComboExpired   -= HandleComboExpired;
    }

    // +10 par hit ennemi direct
    private void HandleHitScore(int amount)
    {
        TotalScore += amount;
        OnScoreChanged?.Invoke(TotalScore);
    }

    // Bonus combo à l'expiration
    private void HandleComboExpired(int finalCombo, int finalRank)
    {
        if (finalCombo <= 0) return;

        float mult   = finalRank >= 0 && finalRank < _rankMultipliers.Length
                       ? _rankMultipliers[finalRank]
                       : 1f;

        int bonus = Mathf.RoundToInt(finalCombo * _baseHitScore * mult);
        TotalScore += bonus;

        Color rankColor = finalRank >= 0 && finalRank < _config.rankColors.Length
                          ? _config.rankColors[finalRank]
                          : Color.white;

        OnScoreChanged?.Invoke(TotalScore);
        OnBonusScoreAdded?.Invoke(bonus, rankColor);
    }
}
