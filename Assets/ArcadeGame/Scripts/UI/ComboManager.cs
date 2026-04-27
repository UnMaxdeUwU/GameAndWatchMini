using System;
using UnityEngine;

/// <summary>
/// Cerveau du système combo.
/// - OnComboWarning : déclenche le clignotement d'avertissement (combo va expirer)
/// - OnComboReset   : déclenche l'animation de fin (le combo A expiré)
/// - ResetCombo()   : appelé par ComboUIFeedback quand l'animation est finie
/// </summary>
public class ComboManager : MonoBehaviour
{
    [SerializeField] private SwordPlayer _swordPlayer;
    [SerializeField] private ComboConfig _config;

    [Tooltip("Secondes avant expiration où le clignotement d'avertissement démarre")]
    [SerializeField] private float _warningTime = 1f;

    public static event Action<int>      OnComboIncreased;
    public static event Action<int, int> OnRankChanged;
    public static event Action           OnComboWarning;   // avertissement : clignote, combo pas encore perdu
    public static event Action<int, int> OnComboExpired;   // (combo final, rank final) : combo perdu
    public static event Action           OnComboReset;     // reset gameplay effectif (appelé par UI)
    public static event Action<int>      OnHitScoreAdded;  // +10 par hit ennemi

    public int CurrentCombo { get; private set; }
    public int CurrentRank  { get; private set; } = -1;

    private float _timeoutTimer;
    private bool  _timerRunning;
    private bool  _warningFired;

    private void OnEnable()  => _swordPlayer.AddCombo += OnHit;
    private void OnDisable() => _swordPlayer.AddCombo -= OnHit;

    private void Update()
    {
        if (!_timerRunning) return;

        _timeoutTimer -= Time.deltaTime;

        // Avertissement clignotement
        if (!_warningFired && _timeoutTimer <= _warningTime)
        {
            _warningFired = true;
            OnComboWarning?.Invoke();
        }

        if (_timeoutTimer <= 0f)
        {
            _timerRunning = false;

            // Sauvegarde avant reset pour le score
            int finalCombo = CurrentCombo;
            int finalRank  = CurrentRank;

            CurrentCombo = 0;
            CurrentRank  = -1;

            // Informe l'UI que le combo a expiré (score bonus + animation rank)
            OnComboExpired?.Invoke(finalCombo, finalRank);
        }
    }

    private void OnHit()
    {
        CurrentCombo++;
        _timeoutTimer = _config.comboTimeout;
        _timerRunning = true;
        _warningFired = false; // remet l'avertissement à zéro pour ce cycle

        int newRank = GetRankForCombo(CurrentCombo);
        if (newRank != CurrentRank)
        {
            CurrentRank = newRank;
            OnRankChanged?.Invoke(CurrentRank, CurrentCombo);
        }

        OnComboIncreased?.Invoke(CurrentCombo);
        OnHitScoreAdded?.Invoke(10); // +10 par hit
    }

    /// <summary>
    /// Appelé par ComboUIFeedback quand toutes les animations sont terminées.
    /// Remet la vitesse du joueur à zéro.
    /// </summary>
    public void ResetCombo()
    {
        OnComboReset?.Invoke();
        _swordPlayer.ResetCooldownAndSpeed();
    }

    private int GetRankForCombo(int combo)
    {
        int rank = -1;
        for (int i = 0; i < _config.rankThresholds.Length; i++)
            if (combo >= _config.rankThresholds[i]) rank = i;
        return rank;
    }
}