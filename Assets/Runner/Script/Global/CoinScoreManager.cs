using System;
using UnityEngine;

/// <summary>
/// Singleton that tracks the total coins collected during a Runner run.
/// Listens to Coin.OnCoinCollected and exposes the current count.
/// </summary>
public class CoinScoreManager : MonoBehaviour
{
    public static CoinScoreManager Instance { get; private set; }

    /// <summary>Fired every time the coin count changes.</summary>
    public static event Action<int> OnScoreChanged;

    public int TotalCoins { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnEnable()
    {
        Coin.OnCoinCollected += HandleCoinCollected;
    }

    private void OnDisable()
    {
        Coin.OnCoinCollected -= HandleCoinCollected;
    }

    private void HandleCoinCollected()
    {
        TotalCoins++;
        OnScoreChanged?.Invoke(TotalCoins);
    }
}
